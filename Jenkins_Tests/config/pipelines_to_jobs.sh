#!/bin/bash

HEADER='<?xml version="1.1" encoding="UTF-8"?>
<flow-definition plugin="workflow-job@1292.v27d8cc3e2602">
    <description>Test</description>
    <keepDependencies>false</keepDependencies>
    <properties/>
    <definition class="org.jenkinsci.plugins.workflow.cps.CpsFlowDefinition" plugin="workflow-cps@3659.v582dc37621d8">
        <script>
'

FOOTER='
        </script>
    <sandbox>false</sandbox>
    </definition>
    <triggers/>
    <disabled>false</disabled>
</flow-definition>'

for pipeline in $(find /var/jenkins_home/jobs -name '*.groovy')
do 
    pipeline_directory=$(dirname ${pipeline})

    echo "${HEADER}" >> ${pipeline_directory}/config.xml
    cat "${pipeline}" >> ${pipeline_directory}/config.xml
    echo "${FOOTER}" >> ${pipeline_directory}/config.xml

    rm -rf ${pipeline}
done

