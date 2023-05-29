import groovy.json.JsonSlurper;
import groovy.json.JsonOutput;

public class Response {
    Integer statusCode
    Object headers
    Object payload

    Response(statusCode, headers, payload) {          
        this.statusCode = statusCode
        this.headers = headers
        this.payload = payload
    }
}

def requestAsJson(String httpMode, String url, String requestBody) {
    List<Map<String, String>> headers = [
        [name: 'Accept', value: 'application/json'],
        [name: 'Content-Type', value: 'application/json']
    ]

    return requestAsJson(httpMode, url, headers, requestBody);
}

def requestAsJson(String httpMode, String url, String authToken, String refreshToken, String requestBody) {
    List<Map<String, String>> headers = [
        [name: 'Accept', value: 'application/json'],
        [name: 'Content-Type', value: 'application/json'],
        [name: 'Authorization', value: "Bearer ${authToken}"],
        [name: 'Cookie', value: "refreshToken=${refreshToken}"]
    ]

    return requestAsJson(httpMode, url, headers, requestBody);
}

def requestAsJson(String httpMode, String url, List<Map<String, String>> requestHeaders, String requestBody) {
    def response = httpRequest ignoreSslErrors: true,
        consoleLogResponseBody: true,
        customHeaders: requestHeaders,
        httpMode: httpMode,
        requestBody: requestBody,
        url: url,
        validResponseCodes: '0:999'

    def responseContent = (response.content == null || response.content.isEmpty()) ? "null" : JsonOutput.prettyPrint(response.content)

    println """
=== REQUEST ===
*** Url *** \n${url}
*** Mode *** \n${httpMode}
*** Headers *** \n${requestHeaders}
*** Body *** \n${JsonOutput.prettyPrint(requestBody)}
\n
=== RESPONSE ===
*** Status code *** \n${response.status}
*** Headers *** \n${response.headers}
*** Payload *** \n${responseContent}
    """

    def jsonSlurper = new JsonSlurper()
    return new Response(response.status, response.headers, jsonSlurper.parseText(responseContent))
}

def request(String httpMode, String url) {
    List<Map<String, String>> headers = [
        [name: 'Accept', value: 'application/json']
    ]

    return request(httpMode, url, headers);
}

def request(String httpMode, String url, String authToken, String refreshToken) {
    List<Map<String, String>> headers = [
        [name: 'Accept', value: 'application/json'],
        [name: 'Authorization', value: "Bearer ${authToken}"],
        [name: 'Cookie', value: "refreshToken=${refreshToken}"]
    ]

    return request(httpMode, url, headers);
}

def request(String httpMode, String url, List<Map<String, String>> requestHeaders) {
    def response = httpRequest ignoreSslErrors: true,
        consoleLogResponseBody: true,
        customHeaders: requestHeaders,
        httpMode: httpMode,
        url: url,
        validResponseCodes: '0:999'

    def responseContent = (response.content == null || response.content.isEmpty()) ? "null" : JsonOutput.prettyPrint(response.content)

    println """
=== REQUEST ===
*** Url *** \n${url}
*** Mode *** \n${httpMode}
*** Headers *** \n${requestHeaders}
\n
=== RESPONSE ===
*** Status code *** \n${response.status}
*** Headers *** \n${response.headers}
*** Payload *** \n${responseContent}
    """

    def jsonSlurper = new JsonSlurper()
    return new Response(response.status, response.headers, jsonSlurper.parseText(responseContent))
}

return this