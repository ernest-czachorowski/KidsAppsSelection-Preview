pipeline {
    agent any
    stages {
        stage('Test API/Auth/Login') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    build job: "TEST_api_auth_login", wait: true
                }
            }
        }
        stage('Test API/Auth/Register') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    build job: "TEST_api_auth_register", wait: true
                }
            }
        }
        stage('Test Basic Braffic') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    build job: "TEST_basic_traffic", wait: true
                }
            }
        }
    }
}