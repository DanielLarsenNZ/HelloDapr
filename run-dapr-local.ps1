# wallets
Start-Process pwsh { -c dapr run --app-id hellodapr-wallets --app-port 5022 --dapr-http-port 3500 --dapr-grpc-port 55500 --resources-path ./components }

# aml
Start-Process pwsh { -c dapr run --app-id hellodapr-aml --app-port 5225 --dapr-http-port 3600 --dapr-grpc-port 55600 --resources-path ./components }

# payments
Start-Process pwsh { -c dapr run --app-id hellodapr-payments --app-port 5236 --dapr-http-port 3700 --dapr-grpc-port 55700 --resources-path ./components }