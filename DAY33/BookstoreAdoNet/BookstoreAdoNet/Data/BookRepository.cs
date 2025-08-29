using System.Data;
using System.Data.SqlClient;
using BookstoreAdoNet.Models;


namespace BookstoreAdoNet.Data
{
    public class BookRepository
    {
        private readonly string _connectionString;


        public BookRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        // 1) Get all books using SqlDataReader (connected)
        public List<Book> GetAllBooks()
        {
            var list = new List<Book>();


            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("SELECT BookId, Title, Author, Price, PublishedDate FROM Books", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var book = new Book
                        {
                            BookId = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Author = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            PublishedDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4)
                        };
                        list.Add(book);
                    }
                }
            }


            return list;
        }


        // 2) Get book by id (using parameterized query)
        public Book? GetBookById(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("SELECT BookId, Title, Author, Price, PublishedDate FROM Books WHERE BookId = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
}internal void AddBook(Book book)
        {
            throw new NotImplementedException();
        }

        internal void DeleteBook(int id)
        {
            throw new NotImplementedException();
        }

        internal void UpdateBook(Book book)
        {
            throw new NotImplementedException();
        }
    }
