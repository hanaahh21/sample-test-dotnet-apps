# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files
COPY . . 

# Restore dependencies and build
RUN dotnet publish -c Release -o /out

# Use a lightweight runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy built application
COPY --from=build /out . 

# Expose port
EXPOSE 8080

# Run application
ENTRYPOINT ["dotnet", "YourAppName.dll"]

# Use the .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

# Set the working directory to /app
WORKDIR /app

# Copy the source files into the container
COPY . .

# Restore dependencies and build the application
RUN dotnet restore && \
    dotnet publish -c Release -o out

# Use a smaller .NET runtime image for the final container
FROM mcr.microsoft.com/dotnet/runtime:8.0

# Set the working directory to /app
WORKDIR /app

# Copy the built application from the builder container
COPY --from=builder /app/out .

# Create a user with a known UID/GID within the range 10000-20000.
# This is required to run the container as a non-root user.
RUN adduser \
    --disabled-password \
    --gecos "" \
    --home "/nonexistent" \
    --shell "/sbin/nologin" \
    --no-create-home \
    --uid 10014 \
    "choreo"

# Use the unprivileged user for running the application
USER 10014

# Set the entrypoint to the built executable
ENTRYPOINT ["./crudapi"]

