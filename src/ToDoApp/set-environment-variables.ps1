$env:RabbitMQ:Uri = "amqp://guest:guest@localhost:5672"
$env:RabbitMQ:Exchange= "future.stack.exchange"
$env:Database:ToDo = "Server=localhost;Uid=root;Pwd=root;Database=ToDoBackend"

Get-ChildItem Env:RabbitMQ:Uri 
Get-ChildItem Env:RabbitMQ:Exchange
Get-ChildItem Env:Database:ToDo 
