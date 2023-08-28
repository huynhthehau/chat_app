pipeline{
    agent any
    environment{
        PATH_PROJECT = '/var/jenkins_home/workspace/gitlab-dotnet-postgresql'
        SONAR_PROJECT_KEY = 'haudtr_chat_app_AYo6AazDEC3terW5sOBA'
        SONAR_TOKEN =  credentials("token-sonarqube")

        MIGRATION_NAME = sh(script: 'echo $(date + %Y%m%d%h%m%s)',returnStdout:true).trim()
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
        stage("test with sonarqube"){
            steps{
                withSonarQubeEnv("SonarQube conenction"){
                    sh "cd $PATH_PROJECT \
                    && docker run --rm -e SONAR_HOST_URL=${env.SONAR_HOST_URL} \
                    -e SONAR_SCANNER_OPTS='-Dsonar.projectKey=$SONAR_PROJECT_KEY' \
                    -e SONAR_TOKEN=$SONAR_TOKEN \
                    -v '.:/usr/src' \
                    sonarsource/sonar-scanner-cli"
                }
            }
        }
    }
}