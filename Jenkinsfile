pipeline {
    agent any

    environment {
        IMAGE_NAME = "mosheilan/hello-world"
        GIT_TAG = ""
    }

    stages {

        stage('Debug Trigger') {
            steps {
                sh '''
                  echo "Actor: ${BUILD_USER}"
                  echo "Event: Git Push"
                  echo "SHA: ${GIT_COMMIT}"
                '''
            }
        }

        stage('Test') {
            steps {
                checkout scm

                sh '''
                  dotnet build
                  dotnet test --no-build --verbosity normal
                '''
            }
        }

        stage('Set Tag') {
            steps {
                checkout scm

                script {
                    // --- Bump version (patch) ---
                    def lastTag = sh(script: "git describe --tags --abbrev=0 || echo 0.0.0", returnStdout: true).trim()
                    def parts = lastTag.tokenize('.').collect { it as int }
                    def newTag = "${parts[0]}.${parts[1]}.${parts[2] + 1}"

                    sh """
                        git config user.email "jenkins@local"
                        git config user.name "Jenkins CI"

                        git tag ${newTag}
                        git push https://${GITHUB_TOKEN}@github.com/${env.GIT_URL.split(':')[1].replace('.git','')} --tags
                    """

                    env.GIT_TAG = newTag
                }

                echo "New tag = ${env.GIT_TAG}"
            }
        }

        stage('Build Docker Image') {
            steps {
                sh '''
                  docker buildx create --use || true
                  docker buildx inspect --bootstrap
                  
                  docker buildx build \
                    -t ${IMAGE_NAME}:${GIT_TAG} \
                    -t ${IMAGE_NAME}:latest \
                    -f HelloWorldApp/Dockerfile \
                    --load .
                  
                  docker save -o image.tar ${IMAGE_NAME}:${GIT_TAG}
                '''
            }

            post {
                success {
                    archiveArtifacts artifacts: 'image.tar', fingerprint: true
                }
            }
        }

        stage('Trivy Scan') {
            steps {
                sh '''
                  docker load -i image.tar

                  trivy image \
                    --exit-code 1 \
                    --ignore-unfixed \
                    --scanners vuln \
                    --vuln-type os,library \
                    --severity CRITICAL,HIGH \
                    --format sarif \
                    --output trivy-results.sarif \
                    ${IMAGE_NAME}:${GIT_TAG}
                '''
            }

            post {
                always {
                    archiveArtifacts artifacts: 'trivy-results.sarif', fingerprint: true
                }
            }
        }

        stage('Deploy & Run') {
            steps {
                withCredentials([usernamePassword(credentialsId: 'dockerhub', usernameVariable: 'USER', passwordVariable: 'PASS')]) {
                    sh '''
                      echo "$PASS" | docker login -u "$USER" --password-stdin

                      docker load -i image.tar
                      docker push ${IMAGE_NAME}:${GIT_TAG}
                      docker push ${IMAGE_NAME}:latest

                      docker run --rm ${IMAGE_NAME}:${GIT_TAG} > output.txt
                      grep "Hello World!" output.txt
                    '''
                }

                echo "Version deployed: ${env.GIT_TAG}"
            }
        }
    }
}
