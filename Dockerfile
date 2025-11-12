FROM mcr.microsoft.com/dotnet/sdk:6.0

WORKDIR /app

# Copy project files
COPY *.csproj ./
RUN dotnet restore

# Copy source code
COPY . ./

# Build the project
RUN dotnet build -c Release --no-restore

# Install Allure if needed for reporting
RUN apt-get update && apt-get install -y default-jdk curl
RUN curl -o allure-2.20.1.tgz -Ls https://github.com/allure-framework/allure2/releases/download/2.20.1/allure-2.20.1.tgz \
    && tar -zxvf allure-2.20.1.tgz -C /opt/ \
    && ln -s /opt/allure-2.20.1/bin/allure /usr/bin/allure

# Run tests
CMD ["dotnet", "test", "--logger:trx", "--results-directory:/app/test-results"]