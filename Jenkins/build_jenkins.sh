#!/bin/bash

docker network create jenkins

docker build -t jenkins_test_infrastructure .