def GENERATE_RANDOM
def RANDOM_TEXT_LEN = 10
def SEND
def TEST_USERNAME_GEN

node {
    GENERATE_RANDOM = load "${env.JENKINS_HOME}/scripts/generateRandom.groovy"
    SEND = load "${env.JENKINS_HOME}/scripts/sendHttp.groovy"
}

node {
    TEST_USERNAME_GEN = GENERATE_RANDOM.textWithNumbers(RANDOM_TEXT_LEN)
}

pipeline {
    agent any

    environment {
        CONFIG = readJSON file: "${env.JENKINS_HOME}/scripts/config.json"
        DEFAULT_PASSWORD = "${CONFIG.default_password}"
        REGISTER_URL = "${CONFIG.register_url}"

        TEST_USERNAME = "${TEST_USERNAME_GEN}"
        TEST_EMAIL = "${TEST_USERNAME_GEN}@google.com"
    }

    stages {
        stage('Clean directory') {
            steps {
                cleanWs()
            }
        }

        stage('Test - Register should pass') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def requestBody="""{
                            "username": "${TEST_USERNAME}",
                            "email": "${TEST_EMAIL}",
                            "password": "${DEFAULT_PASSWORD}",
                            "confirmpassword": "${DEFAULT_PASSWORD}"
                        }"""

                        def response = SEND.requestAsJson('PUT', REGISTER_URL, requestBody);

                        assert response.statusCode == 201;
                        assert response.payload.data != null;
                        assert response.payload.success == true;
                        assert response.payload.message == "Registration successful.";

                        assert response.headers['Set-Cookie'][0].contains('refreshToken') == true;
                        assert response.headers['Set-Cookie'][0].contains('httponly') == true;
                        assert response.headers['Set-Cookie'][0].contains('secure') == true;
                    }
                }
            }
        }

        stage('Test - Register with existing email should fail') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def randomName = GENERATE_RANDOM.textWithNumbers(RANDOM_TEXT_LEN)
                        def requestBody="""{
                            "username": "${randomName}",
                            "email": "${TEST_EMAIL}",
                            "password": "${DEFAULT_PASSWORD}",
                            "confirmpassword": "${DEFAULT_PASSWORD}"
                        }"""

                        def response = SEND.requestAsJson('PUT', REGISTER_URL, requestBody);

                        assert response.statusCode == 409;
                        assert response.payload.data == null;
                        assert response.payload.success == false;
                        assert response.payload.message == "A user with the email: ${TEST_EMAIL} already exists in the database.";
                    }
                }
            }
        }
        
        stage('Test - Register with existing username should fail') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def randomName = GENERATE_RANDOM.textWithNumbers(RANDOM_TEXT_LEN)
                        def requestBody="""{
                            "username": "${TEST_USERNAME}",
                            "email": "${randomName}@google.com",
                            "password": "${DEFAULT_PASSWORD}",
                            "confirmpassword": "${DEFAULT_PASSWORD}"
                        }"""

                        def response = SEND.requestAsJson('PUT', REGISTER_URL, requestBody);

                        assert response.statusCode == 409;
                        assert response.payload.data == null;
                        assert response.payload.success == false;
                        assert response.payload.message == "A user with the username: ${TEST_USERNAME} already exists in the database.";
                    }
                }
            }
        }
        
        stage('Test - Register with invalid email should fail') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def randomName = GENERATE_RANDOM.textWithNumbers(RANDOM_TEXT_LEN)
                        def requestBody="""{
                            "username": "${randomName}",
                            "email": "invalidemail",
                            "password": "${DEFAULT_PASSWORD}",
                            "confirmpassword": "${DEFAULT_PASSWORD}"
                        }"""

                        def response = SEND.requestAsJson('PUT', REGISTER_URL, requestBody);

                        assert response.statusCode == 400;
                        assert response.payload.errors.Email[0] == "Invalid email format.";
                    }
                }
            }
        }
        
        stage('Test - Register with weak password should fail') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def randomName = GENERATE_RANDOM.textWithNumbers(RANDOM_TEXT_LEN)
                        def requestBody="""{
                            "username": "${randomName}",
                            "email": "${randomName}@google.com",
                            "password": "weak",
                            "confirmpassword": "weak"
                        }"""

                        def response = SEND.requestAsJson('PUT', REGISTER_URL, requestBody);

                        assert response.statusCode == 400;
                        assert response.payload.errors.Password[0] == "The field Password must be a string with a minimum length of 8 and a maximum length of 64.";
                    }
                }
            }
        }
        
        stage('Test - Register with empty username should fail') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def randomName = GENERATE_RANDOM.textWithNumbers(RANDOM_TEXT_LEN)
                        def requestBody="""{
                            "username": "",
                            "email": "${randomName}@google.com",
                            "password": "${DEFAULT_PASSWORD}",
                            "confirmpassword": "${DEFAULT_PASSWORD}"
                        }"""

                        def response = SEND.requestAsJson('PUT', REGISTER_URL, requestBody);

                        assert response.statusCode == 400;
                        assert response.payload.errors.Username[0] == "The field Username must be a string with a minimum length of 4 and a maximum length of 16.";
                    }
                }
            }
        }
        
        stage('Test - Register with empty email should fail') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def randomName = GENERATE_RANDOM.textWithNumbers(RANDOM_TEXT_LEN)
                        def requestBody="""{
                            "username": "${randomName}",
                            "email": "",
                            "password": "${DEFAULT_PASSWORD}",
                            "confirmpassword": "${DEFAULT_PASSWORD}"
                        }"""

                        def response = SEND.requestAsJson('PUT', REGISTER_URL, requestBody);

                        assert response.statusCode == 400;
                        assert response.payload.errors.Email[0] == "Invalid email format.";
                    }
                }
            }
        }
        
        stage('Test - Register with empty password should fail') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def randomName = GENERATE_RANDOM.textWithNumbers(RANDOM_TEXT_LEN)
                        def requestBody="""{
                            "username": "${randomName}",
                            "email": "${randomName}@google.com",
                            "password": "",
                            "confirmpassword": ""
                        }"""

                        def response = SEND.requestAsJson('PUT', REGISTER_URL, requestBody);

                        assert response.statusCode == 400;
                        assert response.payload.errors.Password[0] == "The field Password must be a string with a minimum length of 8 and a maximum length of 64.";
                    }
                }
            }
        }     
    }
}

        