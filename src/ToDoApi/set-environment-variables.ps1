$env:RabbitMQ:Uri = "amqp://guest:guest@localhost:5672"
$env:RabbitMQ:Exchange= "future.stack.exchange"
$env:Database:ToDo = "Server=localhost;Uid=root;Pwd=root;Database=ToDoBackend"
$env:Database:MessageStore= "Server=localhost;Uid=root;Pwd=root;Database=ToDoBackend"
$env:Database:MessageTableName= "Messages"

Get-ChildItem Env:RabbitMQ:Uri 
Get-ChildItem Env:RabbitMQ:Exchange
Get-ChildItem Env:Database:ToDo 
Get-ChildItem Env:Database:MessageStore
Get-ChildItem Env:Database:MessageTableName
