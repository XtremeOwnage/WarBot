Certainly! Here is the updated README file with a detailed table and the removal of the unused configurations:

# WarBot Configuration Guide

This guide provides instructions on how to configure the WarBot application when running in a Docker container. You can use environment variables, an `appsettings.json` file, or a combination of both for configuration.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Using Environment Variables](#using-environment-variables)
- [Using appsettings.json](#using-appsettingsjson)
- [Combining Environment Variables and appsettings.json](#combining-environment-variables-and-appsettingsjson)
- [Dockerfile](#dockerfile)
- [Docker Compose](#docker-compose)

## Prerequisites

Ensure you have the following installed:

- Docker
- Docker Compose (optional, for using docker-compose)

## Using Environment Variables

You can configure the application using environment variables. Below is a list of environment variables that you can set:

| Environment Variable           | Description                                                                 |
| ------------------------------ | --------------------------------------------------------------------------- |
| `SUPERADMIN_USER_IDS`          | Comma-separated list of Discord user IDs that will have "god mode" admin access. |
| `ADMIN_GUILDS`                 | Comma-separated list of guild IDs authorized to use "god-mode" commands.    |
| `DB_HOST`                      | The host address for the WarBot database.                                   |
| `DB_PORT`                      | The port number for the WarBot database.                                    |
| `DB_NAME`                      | The name of the WarBot database.                                            |
| `DB_USER`                      | The username for the WarBot database.                                       |
| `DB_PASS`                      | The password for the WarBot database.                                       |
| `DISCORD_TOKEN`                | The token for the Discord bot.                                              |
| `DISCORD_ID`                   | The client ID for the Discord bot.                                          |
| `DISCORD_SECRET`               | The client secret for the Discord bot.                                      |
| `PUBLIC_URL`                   | The public URL of the bot, used to direct users to a website.               |

### Example

```sh
docker run -e SUPERADMIN_USER_IDS="381654208073433091" \
           -e ADMIN_GUILDS="458992709718245377,381654582444687370" \
           -e DB_HOST="your-db-host" \
           -e DB_PORT="3323" \
           -e DB_NAME="your-db-name" \
           -e DB_USER="your-db-user" \
           -e DB_PASS="your-db-pass" \
           -e DISCORD_TOKEN="your-discord-token" \
           -e DISCORD_ID="your-discord-id" \
           -e DISCORD_SECRET="your-discord-secret" \
           -e PUBLIC_URL="https://warbot.dev/" \
           your-docker-image
```

## Using appsettings.json

You can also use an `appsettings.json` file for configuration. Create the `appsettings.json` file with the necessary settings and mount it to the Docker container.

### Example appsettings.json

```json
{
{
  "DetailedErrors": true,
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Warning",
      "Microsoft.EntityFrameworkCore.Infrastructure": "Warning",
      "Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware": "Warning"
    }
  },
  "AllowedHosts": "*",
  "BotConfig": {
    "SUPERADMIN_USER_IDS": [ 381654208073433091 ],
    "ADMIN_GUILDS": [ 458992709718245377, 381654582444687370 ],
    "DB_HOST": "your-db-host",
    "DB_PORT": 3323,
    "DB_NAME": "your-db-name",
    "DB_USER": "your-db-user",
    "DB_PASS": "your-db-pass",
    "DISCORD_TOKEN": "your-discord-token",
    "DISCORD_ID": 0,
    "DISCORD_SECRET": "your-discord-secret",
    "PUBLIC_URL": "https://warbot.dev/"
  }
}

}
```

### Running with appsettings.json

```sh
docker run -v /path/to/your/appsettings.json:/app/appsettings.json \
           your-docker-image
```

## Combining Environment Variables and appsettings.json

You can combine both environment variables and `appsettings.json` for configuration. Environment variables take priority over the settings in `appsettings.json`.

### Example

```sh
docker run -e DB_PASS="your-env-db-pass" \
           -v /path/to/your/appsettings.json:/app/appsettings.json \
           your-docker-image
```

In this example, all settings are read from `appsettings.json`, but the `DB_PASS` is overridden by the environment variable.

## Docker Compose

If you are using Docker Compose, you can define the environment variables and volume mounts in your `docker-compose.yml` file.

### Example docker-compose.yml

```yaml
version: '3.4'

services:
  warbot:
    image: your-docker-image
    environment:
      - DB_PASS=your-env-db-pass
    volumes:
      - /path/to/your/appsettings.json:/app/appsettings.json
    ports:
      - "80:80"
```

## Conclusion

You can configure the WarBot application using either environment variables, an `appsettings.json` file, or a combination of both. Environment variables take precedence over the configuration file settings. Use the methods that best suit your deployment scenario.