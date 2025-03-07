using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;

class Program
{
    static List<Item> items = new List<Item>();
    static void Main()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://*:8080/");
        listener.Start();
        Console.WriteLine("Listening on port 8080...");

        while (true)
        {
            var context = listener.GetContext();
            var request = context.Request;
            var response = context.Response;

            string responseText = "";
            response.ContentType = "application/json";

            if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/items")
            {
                responseText = JsonSerializer.Serialize(items);
            }
            else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/items")
            {
                using var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding);
                var body = reader.ReadToEnd();
                var item = JsonSerializer.Deserialize<Item>(body);
                if (item != null)
                {
                    item.Id = items.Count > 0 ? items.Max(i => i.Id) + 1 : 1;
                    items.Add(item);
                    response.StatusCode = 201;
                }
            }
            else if (request.HttpMethod == "PUT" && request.Url.AbsolutePath.StartsWith("/items/"))
            {
                int id = int.Parse(request.Url.AbsolutePath.Split('/').Last());
                var item = items.FirstOrDefault(i => i.Id == id);
                if (item != null)
                {
                    using var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding);
                    var body = reader.ReadToEnd();
                    var updatedItem = JsonSerializer.Deserialize<Item>(body);
                    if (updatedItem != null)
                    {
                        item.Name = updatedItem.Name;
                        response.StatusCode = 204;
                    }
                }
                else
                {
                    response.StatusCode = 404;
                }
            }
            else if (request.HttpMethod == "DELETE" && request.Url.AbsolutePath.StartsWith("/items/"))
            {
                int id = int.Parse(request.Url.AbsolutePath.Split('/').Last());
                var item = items.FirstOrDefault(i => i.Id == id);
                if (item != null)
                {
                    items.Remove(item);
                    response.StatusCode = 204;
                }
                else
                {
                    response.StatusCode = 404;
                }
            }
            else
            {
                response.StatusCode = 404;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(responseText);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.Close();
        }
    }

    class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
