FROM jenkins/jenkins:lts
ENV JAVA_OPTS -Djenkins.install.runSetupWizard=false
ENV JENKINS_CONFIG /var/jenkins_home/jenkins.yaml
COPY config/plugins.txt /usr/share/jenkins/ref/plugins.txt
RUN jenkins-plugin-cli --plugin-file /usr/share/jenkins/ref/plugins.txt
COPY config/jenkins.yaml /var/jenkins_home/jenkins.yaml
COPY jobs /var/jenkins_home/jobs
COPY scripts /var/jenkins_home/scripts
COPY config/pipelines_to_jobs.sh /var/jenkins_home/pipelines_to_jobs.sh

USER root
# RUN apt-get update && apt-get install -y vim
RUN /var/jenkins_home/pipelines_to_jobs.sh
RUN chown -R jenkins:jenkins /var/jenkins_home/jobs
RUN chown -R jenkins:jenkins /var/jenkins_home/scripts
USER jenkins
