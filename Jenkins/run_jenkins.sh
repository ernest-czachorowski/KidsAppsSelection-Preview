#!/bin/bash

docker run \
    --detach \
    --name jenkins \
    --env JENKINS_ADMIN_ID=admin \
    --env JENKINS_ADMIN_PASSWORD=jenkins \
    --env DOCKER_TLS_CERTDIR=/certs \
    --env DOCKER_TLS_VERIFY=1 \
    --network jenkins \
    --network-alias docker \
    --volume jenkins-docker-certs:/certs/client \
    --volume jenkins-data:/var/jenkins_home \
    --publish 8080:8080 \
    --publish 50000:50000 \
    jenkins_test_infrastructure