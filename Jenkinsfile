pipeline{
    agent any
    environment{
        PATH_PROJECT = '/var/jenkins_home/workspace/gitlab-dotnet-postgresql'
        SONAR_PROJECT_KEY = 'haudtr_chat_app_AYo6NCz_wGp5cFdDrzWG'
        SONAR_TOKEN =  credentials("sonarqube-10h30")

        MIGRATION_NAME = sh(script: 'echo $(date + %Y%m%d%h%m%s)',returnStdout:true).trim()


        DOCKER_HUB = 'haudtr285'
        DOCKERHUB_CREDENTIALS = credentials('dockerhub')
        NAME_BACKEND = 'chat_app'
        DOCKER_TAG="${GIT_BRANCH.tokenize('/').pop()}-${GIT_COMMIT.substring(0,7)}"
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
        // stage("test with sonarqube"){
        //     steps{
        //         withSonarQubeEnv("SonarQube conenction"){
        //             sh "cd $PATH_PROJECT \
        //             && docker run --network=jenkins --rm -e SONAR_HOST_URL=${env.SONAR_HOST_URL} \
        //             -e SONAR_SCANNER_OPTS='-Dsonar.projectKey=$SONAR_PROJECT_KEY' \
        //             -e SONAR_TOKEN=$SONAR_TOKEN \
        //             -v '.:/usr/src' \
        //             sonarsource/sonar-scanner-cli"
        //         }
        //     }
        // }
        stage('Migration database') {
            steps {
                script {
                    try {
                        timeout(time: 5, unit: 'MINUTES') {
                            env.userChoice = input message: 'Do you want to migrate the database?',
                                parameters: [choice(name: 'Versioning Service', choices: 'no\nyes', description: 'Choose "yes" if you want to migrate!')]
                        }
                        if (env.userChoice == 'yes') {
                            echo "Migration success!"
                            // sh "cd $PATH_PROJECT/BE \
                            // && docker run --rm -v .:/app -w /app dotnet6-app dotnet ef migrations add $MIGRATION_NAME \
                            // && docker run --rm -v .:/app -w /app dotnet6-app dotnet ef database update"
                        } else {
                            echo "Migration cancelled."
                        }
                    } catch (Exception err){
                        def user = err.getCauses()[0].getUser()
                        if ('SYSTEM' == user.toString()) {
                            def didTimeout = true
                            echo "Timeout. Migration cancelled."
                        } else {
                            echo "Migration cancelled by: ${user}"
                        }
                    }
                }
            }
        }
        stage('Build and push images') {
            steps {
                script {
                    env.IMAGE_TAG = DOCKER_TAG
                    sh "cd $PATH_PROJECT \
                    && IMAGE_TAG=${IMAGE_TAG} \
                    && NAME_BACKEND=${NAME_BACKEND} \
                    && docker-compose build --parallel \
                    && docker tag ${NAME_BACKEND}:$DOCKER_TAG ${DOCKER_HUB}/${NAME_BACKEND}:$DOCKER_TAG \
                    && echo $DOCKERHUB_CREDENTIALS_PSW | docker login -u $DOCKERHUB_CREDENTIALS_USR --password-stdin \
                    && docker push ${DOCKER_HUB}/${NAME_BACKEND}:$DOCKER_TAG \
                    && docker rmi ${DOCKER_HUB}/${NAME_BACKEND}:$DOCKER_TAG"
                }
            }
        }
    }
}