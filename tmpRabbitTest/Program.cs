using RabbitMQ.Client; class Program { static void Main() { var factory = new ConnectionFactory(); var conn = factory.CreateConnection(); } }
