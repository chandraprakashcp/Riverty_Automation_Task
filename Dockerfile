FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app

# Copy project files first
COPY *.csproj ./
RUN dotnet restore

# Copy source code
COPY . .

# Build
RUN dotnet build --configuration Release

# Create reports directory
RUN mkdir -p /app/test-results

# Run tests with explicit results directory
CMD dotnet test --configuration Release --logger "trx;logfilename=testresults.trx" --results-directory:/app/test-results