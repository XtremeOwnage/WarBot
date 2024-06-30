# WarBot Helm Chart

This repository contains the Helm chart for deploying the WarBot application. The WarBot Helm chart includes configuration for deploying the WarBot bot and optionally the WarBot UI along with necessary services and configurations.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Adding the Helm Repository](#adding-the-helm-repository)
- [Installing the Chart](#installing-the-chart)
- [Configuration](#configuration)
- [Uninstalling the Chart](#uninstalling-the-chart)

## Prerequisites

- Kubernetes 1.12+
- Helm 3.0+
- A configured Kubernetes cluster
- `kubectl` and `helm` installed and configured on your local machine

## Adding the Helm Repository

To add the Helm repository, run the following commands:

```sh
helm repo add warbot https://XtremeOwnage.github.io/WarBot/
helm repo update
```

## Installing the Chart

To install the chart with the release name `my-warbot`:

```sh
helm install my-warbot warbot/warbot -f custom-values.yaml
```

## Configuration

The following table lists the configurable parameters of the WarBot chart and their default values.

| Parameter                      | Description                                                                                      | Default                           |
|--------------------------------|--------------------------------------------------------------------------------------------------|-----------------------------------|
| `replicaCount`                 | Number of WarBot replicas                                                                        | `1`                               |
| `image.repository`             | WarBot image repository                                                                          | `git.kube.xtremeownage.com/xtremeownage.com/warbot` |
| `image.tag`                    | WarBot image tag                                                                                 | `latest`                          |
| `image.pullPolicy`             | Image pull policy                                                                               | `IfNotPresent`                    |
| `ui.enabled`                   | Enable or disable the WarBot UI deployment                                                       | `true`                            |
| `ui.image.repository`          | WarBot UI image repository                                                                       | `git.kube.xtremeownage.com/xtremeownage.com/warbot-ui` |
| `ui.image.tag`                 | WarBot UI image tag                                                                              | `latest`                          |
| `ui.image.pullPolicy`          | UI Image pull policy                                                                             | `IfNotPresent`                    |
| `database.enabled`             | Enable or disable the provided database                                                          | `true`                            |
| `database.host`                | Database host name                                                                               | `db`                              |
| `database.port`                | Database port number                                                                             | `3306`                            |
| `database.name`                | Database name                                                                                    | `warbot`                          |
| `database.user`                | Database username                                                                                | `warbot_user`                     |
| `database.password`            | Database password                                                                                | `secret`                          |
| `externalDatabase.host`        | External database host name (if not using provided database)                                     | `""`                              |
| `externalDatabase.port`        | External database port number (if not using provided database)                                   | `3306`                            |
| `externalDatabase.name`        | External database name (if not using provided database)                                          | `""`                              |
| `externalDatabase.user`        | External database username (if not using provided database)                                      | `""`                              |
| `externalDatabase.password`    | External database password (if not using provided database)                                      | `""`                              |
| `discord.id`                   | Discord bot client ID                                                                            | `"437983722193551363"`            |
| `discord.token`                | Discord bot token                                                                                | `"your-discord-token"`            |
| `discord.secret`               | Discord bot client secret                                                                        | `"your-discord-secret"`           |
| `publicUrl`                    | Public URL of the bot                                                                            | `"https://warbot.dev/"`           |
| `service.type`                 | Kubernetes service type                                                                          | `ClusterIP`                       |
| `service.port`                 | Kubernetes service port                                                                          | `80`                              |
| `uiService.type`               | Kubernetes UI service type                                                                       | `ClusterIP`                       |
| `uiService.port`               | Kubernetes UI service port                                                                       | `80`                              |
| `ingress.enabled`              | Enable or disable ingress                                                                        | `false`                           |
| `ingress.annotations`          | Ingress annotations                                                                              | `{}`                              |
| `ingress.hosts`                | Ingress hosts                                                                                    | `[ { host: "chart-example.local", paths: [] } ]` |
| `ingress.tls`                  | Ingress TLS configuration                                                                        | `[]`                              |
| `ingressRoute.enabled`         | Enable or disable Traefik IngressRoute                                                           | `false`                           |
| `ingressRoute.entryPoints`     | Traefik IngressRoute entry points                                                                | `[ "websecure" ]`                 |
| `ingressRoute.routes`          | Traefik IngressRoute routes                                                                      | `[ { match: "Host(`warbot.dev`)", kind: "Rule", services: [ { name: "web", port: "http" } ] } ]` |
| `networkPolicies.enabled`      | Enable or disable network policies                                                               | `true`                            |
| `networkPolicies.policies`     | List of network policies                                                                         | (see `values.yaml`)               |
| `resources`                    | Resource requests and limits for the containers                                                  | `{}`                              |
| `autoscaling`                  | Autoscaling configuration                                                                        | `{}`                              |
| `nodeSelector`                 | Node selector configuration                                                                      | `{}`                              |
| `tolerations`                  | Tolerations configuration                                                                        | `[]`                              |
| `affinity`                     | Affinity configuration                                                                           | `{}`                              |
| `env.SUPERADMIN_USER_IDS`      | Comma-separated list of super admin user IDs                                                     | `"381654208073433091"`            |
| `env.ADMIN_GUILDS`             | Comma-separated list of admin guild IDs                                                          | `"458992709718245377,381654582444687370"` |

## Uninstalling the Chart

To uninstall the `my-warbot` deployment:

```sh
helm uninstall my-warbot
```

This command removes all the Kubernetes components associated with the chart and deletes the release.

## Conclusion

You can configure the WarBot Helm chart using the `values.yaml` file or by providing a custom `values.yaml` file. The chart includes options for deploying the WarBot bot and UI, managing database configurations, and setting up network policies.
```

### Step 5: Add the Helm Repository

Once the workflow runs and publishes the chart, users can add your Helm repository with the following commands:

```sh
helm repo add warbot https://XtremeOwnage.github.io/WarBot/
helm repo update
```