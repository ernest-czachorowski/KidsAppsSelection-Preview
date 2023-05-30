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

        DELETE_USER_URL = "${CONFIG.delete_user_url}"
        LOGIN_URL = "${CONFIG.login_url}"
        LOGOUT_URL = "${CONFIG.logout_url}"
        REFRESH_TOKEN_URL = "${CONFIG.refresh_token_url}"
        REGISTER_URL = "${CONFIG.register_url}"
        MY_PROFILE_URL = "${CONFIG.my_profile_url}"

        TEST_USERNAME = "${TEST_USERNAME_GEN}"
        TEST_EMAIL = "${TEST_USERNAME_GEN}@google.com"

        AUTH_TOKEN = ""
        REFRESH_TOKEN = ""
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

                        AUTH_TOKEN = response.payload.data;
                        REFRESH_TOKEN = response.headers['Set-Cookie'][0].split(';').find { it.startsWith('refreshToken=') }.substring('refreshToken='.length())
                    }
                }
            }
        }

        stage('Test - My profile should pass - 0') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def response = SEND.request('GET', MY_PROFILE_URL, AUTH_TOKEN, REFRESH_TOKEN);

                        assert response.statusCode == 200;
                        assert response.payload.data != null;
                        assert response.payload.success == true;
                        assert response.payload.message == "The requested data has been successfully loaded.";
                    }
                }
            }
        }

        stage('Wait - 0') {
            steps {
                sleep(time: 3, unit: "SECONDS")
            }
        }

        stage('Test - Logout should pass') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def response = SEND.request('POST', "${LOGOUT_URL}", AUTH_TOKEN, REFRESH_TOKEN);

                        assert response.statusCode == 200;
                        assert response.payload.data == true;
                        assert response.payload.success == true;
                        assert response.payload.message == "Token blocked.";

                        assert response.headers['Set-Cookie'][0].contains('refreshToken') == true;
                        REFRESH_TOKEN = response.headers['Set-Cookie'][0].split(';').find { it.startsWith('refreshToken=') }.substring('refreshToken='.length())
                    }
                }
            }
        }

        stage('Wait - 1') {
            steps {
                sleep(time: 3, unit: "SECONDS")
            }
        }

        stage('Test - My profile should fail - 1') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def response = SEND.request('GET', MY_PROFILE_URL, AUTH_TOKEN, REFRESH_TOKEN);

                        assert response.statusCode == 401;
                        assert response.payload == null;
                        assert response.headers['WWW-Authenticate'][0] == "Bearer error=\"invalid_token\"";
                    }
                }
            }
        }

        stage('Test - Login should pass') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {               
                        def requestBody="""{
                            "email": "${TEST_EMAIL}", 
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

                        AUTH_TOKEN = response.payload.data;
                        REFRESH_TOKEN = response.headers['Set-Cookie'][0].split(';').find { it.startsWith('refreshToken=') }.substring('refreshToken='.length())
                    }
                }
            }
        }

        stage('Test - My profile should pass - 2') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def response = SEND.request('GET', MY_PROFILE_URL, AUTH_TOKEN, REFRESH_TOKEN);

                        assert response.statusCode == 200;
                        assert response.payload.data != null;
                        assert response.payload.success == true;
                        assert response.payload.message == "The requested data has been successfully loaded.";
                    }
                }
            }
        }

        stage('Wait - 2') {
            steps {
                sleep(time: 3, unit: "SECONDS")
            }
        }

        stage('Test - Refresh token should pass') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def requestBody="""{
                            "email": "${TEST_EMAIL}", 
                            "password": "${DEFAULT_PASSWORD}"
                        }"""

                        def response = SEND.request('POST', "${REFRESH_TOKEN_URL}/?request=true", AUTH_TOKEN, REFRESH_TOKEN);

                        assert response.statusCode == 201;
                        assert response.payload.data != null;
                        assert response.payload.data != AUTH_TOKEN;
                        assert response.payload.success == true;
                        assert response.payload.message == "Token refreshed.";

                        assert response.headers['Set-Cookie'][0].contains('refreshToken') == true;
                        assert response.headers['Set-Cookie'][0].contains('httponly') == true;
                        assert response.headers['Set-Cookie'][0].contains('secure') == true;

                        AUTH_TOKEN = response.payload.data;
                        REFRESH_TOKEN = response.headers['Set-Cookie'][0].split(';').find { it.startsWith('refreshToken=') }.substring('refreshToken='.length())
                    }
                }
            }
        }

        stage('Test - My profile should pass - 3') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def response = SEND.request('GET', MY_PROFILE_URL, AUTH_TOKEN, REFRESH_TOKEN);

                        assert response.statusCode == 200;
                        assert response.payload.data != null;
                        assert response.payload.success == true;
                        assert response.payload.message == "The requested data has been successfully loaded.";
                    }
                }
            }
        }

        stage('Test - Delete user should pass') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {
                        def requestBody="""{
                            "email": "${TEST_EMAIL}",
                            "password": "${DEFAULT_PASSWORD}"
                        }"""

                        def response = SEND.requestAsJson('POST', DELETE_USER_URL, AUTH_TOKEN, REFRESH_TOKEN, requestBody);

                        assert response.statusCode == 200;
                        assert response.payload.data == true;
                        assert response.payload.success == true;
                        assert response.payload.message == "Your account has been deleted.";
                    }
                }
            }
        }

        stage('Test - Login to delete account should fail') {
            steps {
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    script {               
                        def requestBody="""{
                            "email": "${TEST_EMAIL}", 
                            "password": "${DEFAULT_PASSWORD}"
                        }"""

                        def response = SEND.requestAsJson('POST', LOGIN_URL, requestBody);

                        assert response.statusCode == 404;
                        assert response.payload.success == false;
                        assert response.payload.message == "The requested user with email: ${TEST_EMAIL} could not be found in the database.";
                    }
                }
            }
        }
    }
}