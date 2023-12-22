#  Hello DAPR


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

* <https://docs.dapr.io/developing-applications/building-blocks/state-management/state-management-overview/>
