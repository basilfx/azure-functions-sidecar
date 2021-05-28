# Azure Functions Sidecar
A sidecar container for your Azure Functions deployments in Kubernetes.

## Introduction
This sidecar application can run next to a Azure Functions deployment in
Kubernetes, to provide startup, liveness and readiness information. A function
host does not provide this information.

While similar functionality could be implemented using a `HttpTrigger`, it is
quite cumbersome. Similar functionality has [not][2] yet been provided by the
function host.

Running as a sidecar has the benefit that it is easy to add, and does not
interfere with the original function deployment. It can directly access the
container, because they both share the same network.

## Features
* Provide startup, liveness and readiness endpoints
* Provide metrics endpoint for function metrics

## Installation
Modify your deployments to include the additional sidecar container. The
sidecar needs to have the master key to access the function host API.

```yaml
apiVersion: apps/v1
kind: Deployment
spec:
  ..
  template:
    ..
    spec:
      ..
      containers:
        - ... (your function deployment)
        - name: azure-functions-sidecar
          image: (image)
          env:
            - name: Logging__LogLevel__Default
              value: Warning
            - name: Function__Host
              value: http://localhost
            - name: Function__Key
              valueFrom:
                secretKeyRef:
                  name: function-keys-secret
                  key: host.master
          ports:
            - name: http
              containerPort: 5000
          startupProbe:
            httpGet:
              path: /startupz
              port: http
            # ASP.NET first request might be slow, change if needed.
            # timeoutSeconds: 5
          livenessProbe:
            httpGet:
              path: /livez
              port: http
          readinessProbe:
            httpGet:
              path: /readyz
              port: http
          resources:
            limits:
              cpu: 100m
              memory: 128M
            requests:
              cpu: 50m
              memory: 64M
```

Configure additional probe thresholds if desired. Refer to the Kubernetes
documentation for more information.

When any of the probes fail, the whole Pod will be restarted.

### Dependenies
This project depends on the [azure-functions-host][1] project as a Git
submodule, in order to have the API models available (there is no Nuget
package). After cloning this repository, ensure to run
`git submodule update --init --recursive` to fetch this dependency as well.

### Preparation
Ensure that you have the .NET Core 3.1 SDK installed. Install Docker to build
the container.

### Compilation
Run `dotnet build` to build the application. You can run it using `dotnet run`
from the `src/Functions.Sidecar` folder. Make sure you have a initialized
copy of `appsettings.json` (template included).

## License
See the [`LICENSE.md`](LICENSE.md) file (MIT license).

[1]: https://github.com/Azure/azure-functions-host
[2]: https://github.com/Azure/azure-functions-host/issues/5259
