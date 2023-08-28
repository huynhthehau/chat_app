pipeline{
    agent any
    environment{
        PATH_PROJECT = '/var/jenkins_home/workspace/gitlab-dotnet-postgresql'
    }
    stages{
        // stage('Check source'){
        //     steps{
        //         // sh 'cp -r . $PATH_PROJECT'

        //     }
        // }
        stage('test with dotnet'){
            steps{
                sh "cd $PATH_PROJECT \
                && docker build -t dotnet6-app -f Dockerfile.dotnet6 . \
                && docker run --rm -v .:/app -w /app dotnet6-app dotnet test"
            }
        }
    }
}