# .NET Application CI/CD Workflow

This repository contains a comprehensive GitHub Actions workflow that builds, tests, scans, and deploys a .NET application using Docker. The workflow demonstrates modern DevOps practices including automated testing, vulnerability scanning with Trivy, semantic versioning, and container deployment to Docker Hub.

## Workflow Overview

The pipeline consists of five sequential jobs that ensure code quality and security before deployment:

1. **Debug Job**: Displays workflow trigger information including actor, event type, and commit SHA for troubleshooting purposes
2. **Test Job**: Sets up .NET 8.0 environment, builds the application, and runs unit tests to verify code functionality
3. **Set-Tag Job**: Automatically generates semantic version tags using conventional commits and creates GitHub releases with changelog
4. **Build Job**: Creates Docker image using BuildKit, extracts metadata, and saves the image as an artifact for downstream jobs
5. **Trivy Job**: Downloads the built image and performs comprehensive vulnerability scanning for critical and high-severity issues
6. **Deploy-and-Run Job**: Pushes the validated image to Docker Hub and performs a smoke test to verify the application runs correctly

## Prerequisites

Before running this workflow locally or using the produced images, ensure you have the following tools installed:

- **Docker**: For building and running containerized applications
- **Act**: GitHub Actions local runner for testing workflows locally
- **.NET 8.0 SDK**: Required for building and testing the application
- **Git**: For version control and accessing the repository

## Running the Pipeline Locally with Act

Act allows you to test GitHub Actions workflows on your local machine without pushing to GitHub. This is invaluable for debugging workflow issues and testing changes before committing.

### Installation and Setup

First, install Act following the official documentation for your operating system. On linux, you can use nix:

```bash
sudo apt install nix-bin
```

Please choose the Medium size image

### Local Execution

To run the entire workflow locally, navigate to your repository root and execute:

```bash
# Run the complete workflow
sudo nix --extra-experimental-features flakes --extra-experimental-features nix-command run nixpkgs#act

# Run specific jobs for targeted testing
sudo nix --extra-experimental-features flakes --extra-experimental-features nix-command run nixpkgs#act -- -j test
# Run only the test job
sudo nix --extra-experimental-features flakes --extra-experimental-features nix-command run nixpkgs#act -- -j build
# Run only the build job
sudo nix --extra-experimental-features flakes --extra-experimental-features nix-command run nixpkgs#act -- -j Trivy
# Run only security scanning
```

### Important Local Testing Considerations

When running locally with Act, be aware of these key differences from the GitHub environment:

- **Secrets Management**: Create a `.secrets` file in your repository root containing required secrets like `DOCKERHUB_TOKEN` and `MYGITHUBTOKEN`
- **Environment Variables**: The workflow uses `vars.DOCKERHUB_USER` which should be defined in your GitHub repository variables
- **Docker Context**: Ensure Docker daemon is running and accessible to Act
- **File Paths**: The Dockerfile path references `../../HelloWorldApp/Dockerfile` - verify this path exists relative to your workflow file

Example `.secrets` file format:

```
DOCKERHUB_TOKEN=your_dockerhub_token_here
MYGITHUBTOKEN=your_github_token_here
GITHUB_TOKEN=your_github_token_here
```

## Pulling and Running the Docker Image

Once the workflow completes successfully, the Docker image is available on Docker Hub for deployment and testing.

### Pulling the Image

The workflow builds images with semantic version tags. To pull the latest version:

```bash
# Pull the latest tagged version
docker pull mosheilan/hello-world:latest

# Pull a specific version (replace with actual tag)
docker pull mosheilan/hello-world:v1.2.3
```

### Running the Container

The application appears to be a simple "Hello World" application based on the smoke test in the workflow:

```bash
# Run the container and see output
docker run --rm mosheilan/hello-world:latest

# Run in interactive mode for debugging
docker run -it mosheilan/hello-world:latest /bin/bash
```

### Verifying the Application

The workflow includes a smoke test that checks for "Hello World!" output. You can verify your local run produces the expected output:

```bash
# Capture and verify output
docker run --rm mosheilan/hello-world:latest > output.txt
grep "Hello World!" output.txt && echo "Application working correctly"
```

## Successful Workflow Examples

You can view successful workflow runs and their outputs by navigating to the Actions tab in your GitHub repository. Look for runs with green checkmarks that demonstrate the complete pipeline execution from code commit to image deployment.