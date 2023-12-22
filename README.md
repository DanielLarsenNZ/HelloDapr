#  Hello DAPR

An example of an ASP.NET application and the Dapr .NET SDK.

## Pros and gotchas

* GRPC built in and by default.
* mTLS and Client encryption built in. 
* OpenAPI
* Transactional outbox pattern built-in.
* Async / await + cancellation tokens
* Retry built-in.
* Highly opinionated API for distributed computing.
* PUTS will upsert and this behaviour cannot be overidden, even at the API level.

### Simple concurrency and consistency models

* ConcurrencyMode: FirstWrite or LastWrite
* ConsistencyMode: Strong or Consistent

##  Getting started

Install Dapr CLI (Windows) and initialize Dapr slim.

```powershell
winget install Dapr.CLI
dapr init --slim
```

Create component YAMLs, update connection strings and keys.

```powershell
copy ./components/pubsub.yaml.example ./components/pubsub.yaml
copy ./components/statestore.yaml.example ./statestore/pubsub.yaml
```

Start the DAPR apps

```powershell
./run-dapr-local.ps1
```

Now run the Visual Studio solution (./src/HelloDapr.sln) - start all projects.

## Helpful docs

* Dapr Slim: <https://docs.dapr.io/operations/hosting/self-hosted/self-hosted-no-docker/>
* Dapr Ebook is very good: <https://github.com/dotnet-architecture/eBooks/blob/1ed30275281b9060964fcb2a4c363fe7797fe3f3/current/dapr-for-net-developers/Dapr-for-NET-Developers.pdf>
* <https://docs.dapr.io/developing-applications/building-blocks/state-management/state-management-overview/>
* <https://xaviergeerinck.com/2020/08/07/debugging-common-dapr-issues/>