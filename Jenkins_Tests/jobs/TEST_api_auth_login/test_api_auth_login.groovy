def SEND

node {
    SEND = load "${env.JENKINS_HOME}/scripts/sendHttp.groovy"
}

pipeline {
    agent any

    environment {
        CONFIG = readJSON file: "${env.JENKINS_HOME}/scripts/config.json"

        ADMIN_EMAIL = "${CONFIG.admin_1_email}"
        DEFAULT_PASSWORD = "${CONFIG.default_password}"
        LOGIN_URL = "${CONFIG.login_url}"
    }

    stages {
        stage('Clean directory') {
            steps {
                cleanWs()
            }
        }

        stage('Test - Login should pass') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def requestBody="""{
                            "email": "${ADMIN_EMAIL}", 
                            "password": "${DEFAULT_PASSWORD}"
                        }"""

                        def response = SEND.requestAsJson('POST', LOGIN_URL, requestBody);

                        assert response.statusCode == 200;
                        assert response.payload.data != null;
                        assert response.payload.success == true;
                        assert response.payload.message == "Token created.";

                        assert response.headers['Set-Cookie'][0].contains('refreshToken') == true;
                        assert response.headers['Set-Cookie'][0].contains('httponly') == true;
                        assert response.headers['Set-Cookie'][0].contains('secure') == true;
                    }
                }
            }
        }

        stage('Test - Login with wrong email should fail') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def requestBody="""{
                            "email": "wrong_email_example@google.com",
                            "password": "${DEFAULT_PASSWORD}"
                        }"""

                        def response = SEND.requestAsJson('POST', LOGIN_URL, requestBody);

                        assert response.statusCode == 404;
                        assert response.payload.success == false;
                        assert response.payload.message == "The requested user with email: wrong_email_example@google.com could not be found in the database.";
                    }
                }
            }
        }
        
        stage('Test - Login with wrong password should fail') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def requestBody="""{
                            "email": "${ADMIN_EMAIL}", 
                            "password": "wrong_password_example"
                        }"""

                        def response = SEND.requestAsJson('POST', LOGIN_URL, requestBody);

                        assert response.statusCode == 401;
                        assert response.payload.success == false;
                        assert response.payload.message == "Wrong password.";
                    }
                }
            }
        }
        
        stage('Test - Login with incorrect email should fail') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def requestBody="""{
                            "email": "incorrect_email_example", 
                            "password": "${DEFAULT_PASSWORD}"
                        }"""

                        def response = SEND.requestAsJson('POST', LOGIN_URL, requestBody);

                        assert response.statusCode == 400;
                        assert response.payload.errors.Email[0] == "Invalid email format.";
                    }
                }
            }
        }
        
        stage('Test - Login with white characters email should fail') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def requestBody="""{
                            "email": "admin- 1@google.com", 
                            "password": "${DEFAULT_PASSWORD}"
                        }"""

                        def response = SEND.requestAsJson('POST', LOGIN_URL, requestBody);

                        assert response.statusCode == 400;
                        assert response.payload.errors.Email[0] == "Field must not contain white characters.";
                    }
                }
            }
        }
        
        stage('Test - Login with white characters password should fail') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def requestBody="""{
                            "email": "${ADMIN_EMAIL}", 
                            "password": "password with white characters"
                        }"""

                        def response = SEND.requestAsJson('POST', LOGIN_URL, requestBody);

                        assert response.statusCode == 400;
                        assert response.payload.errors.Password[0] == "Field must not contain white characters.";
                    }
                }
            }
        }
        
        stage('Test - Login with empty email should fail') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def requestBody="""{
                            "email": "", 
                            "password": "${DEFAULT_PASSWORD}"
                        }"""

                        def response = SEND.requestAsJson('POST', LOGIN_URL, requestBody);

                        assert response.statusCode == 400;
                        assert response.payload.errors.Email[0] == "The field Email must be a string with a minimum length of 8 and a maximum length of 64.";
                    }
                }
            }
        }
        
        stage('Test - Login with empty password should fail') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def requestBody="""{
                            "email": "${ADMIN_EMAIL}", 
                            "password": ""
                        }"""

                        def response = SEND.requestAsJson('POST', LOGIN_URL, requestBody);

                        assert response.statusCode == 400;
                        assert response.payload.errors.Password[0] == "The field Password must be a string with a minimum length of 8 and a maximum length of 64.";
                    }
                }
            }
        }
        
        stage('Test - Login with emoji email should fail') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def requestBody="""{
                            "email": "😀😀😀😀😀😀😀😀@google.com", 
                            "password": "${DEFAULT_PASSWORD}"
                        }"""

                        def response = SEND.requestAsJson('POST', LOGIN_URL, requestBody);

                        assert response.statusCode == 400;
                        assert response.payload.errors.Email[0] == "Invalid email format.";
                    }
                }
            }
        }
        
        stage('Test - Login with emoji password should fail') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def requestBody="""{
                            "email": "${ADMIN_EMAIL}", 
                            "password": "😀😀😀😀😀😀😀😀"
                        }"""

                        def response = SEND.requestAsJson('POST', LOGIN_URL, requestBody);

                        assert response.statusCode == 400;
                        assert response.payload.errors.Password[0] == "Field must not contain emoji.";
                    }
                }
            }
        }
    }
}