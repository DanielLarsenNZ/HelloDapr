#  Hello DAPR


## Pros and gotchas

* GRPC built in and by default.
* Client encryption built in. 
* Highly opinionated API for distributed computing.
* PUTS will upsert and this behaviour cannot be overidden, even at the API level.

### Simple concurrency and consistency models

* ConcurrencyMode: FirstWrite or LastWrite
* ConsistencyMode: Strong or Consistent

##  Getting started

```
cd [this folder]
dapr run --app-id hellodapr --dapr-http-port 3500 --dapr-grpc-port 55417 --resources-path ./components
```

## Helpful docs

* <https://docs.dapr.io/developing-applications/building-blocks/state-management/state-management-overview/>
