using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using WebChatApp.Data;
using WebChatApp.Data.Entities;
using WebChatApp.Hubs;
using WebChatApp.Models;

namespace WebChatApp.Controllers
{
    public class UploadController : BaseController
    {
        private readonly int FileSizeLimit;
        private readonly string[] AllowedExtensions;
        private readonly ManageAppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly IHubContext<ChatHub> _hubContext;

        public UploadController(ManageAppDbContext context,
            IMapper mapper,
            IWebHostEnvironment environment,
            IConfiguration configruation,
            IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _mapper = mapper;
            _environment = environment;
            this._hubContext = hubContext;
            FileSizeLimit = configruation.GetSection("FileUpload").GetValue<int>("FileSizeLimit");
            AllowedExtensions = configruation.GetSection("FileUpload").GetValue<string>("AllowedExtensions").Split(",");
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload([FromForm] UploadViewModel uploadViewModel)
        {
            if (ModelState.IsValid)
            {
                if (!Validate(uploadViewModel.File))
                {
                    return BadRequest("Validation failed!");
                }

                var fileName = DateTime.Now.ToString("yyyymmddMMss") + "_" + Path.GetFileName(uploadViewModel.File.FileName);
                var folderPath = Path.Combine(_environment.WebRootPath, "uploads");
                var filePath = Path.Combine(folderPath, fileName);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadViewModel.File.CopyToAsync(fileStream);
                }

                var user = _context.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                var room = _context.Rooms.Where(r => r.Id == uploadViewModel.RoomId).FirstOrDefault();

                if (user == null || room == null)
                    return NotFound();

                string htmlImage = string.Format(
                    "<a href=\"/uploads/{0}\" target=\"_blank\">" +
                    "<img src=\"/uploads/{0}\" class=\"post-image\">" +
                    "</a>", fileName);

                var message = new Message()
                {
                    Content = Regex.Replace(htmlImage, @"(?i)<(?!img|a|/a|/img).*?>", string.Empty),
                    Timestamp = DateTime.UtcNow,
                    FromUser = user,
                    ToRoom = room
                };

                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();

                // Send image-message to group
                var messageViewModel = _mapper.Map<Message, MessageViewModel>(message);
                await _hubContext.Clients.Group(room.Name).SendAsync("newMessage", messageViewModel);

                return Ok();
            }

            return BadRequest();
        }

        private bool Validate(IFormFile file)
        {
            if (file.Length > FileSizeLimit)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Any(s => s.Contains(extension)))
                return false;

            return true;
        }
    }
}
