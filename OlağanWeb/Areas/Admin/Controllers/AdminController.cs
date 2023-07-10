using Microsoft.AspNetCore.Mvc;
using OlağanWeb.LoginService;
using OlağanWeb.Models;
using System.Data.SqlClient;


namespace OlağanWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class AdminController : Controller
{
    readonly IConfiguration _configuration;
    readonly ILoginControl _loginControl;
    public AdminController(IConfiguration configuration, ILoginControl loginControl)
    {
        _configuration = configuration;
        _loginControl = loginControl;
    }
    [Route("/")]
    public IActionResult Index()
    {
        if (_loginControl.Control(HttpContext.Session.GetString("Giris")))
        {
            return RedirectToAction("login");
        }
        List<TextModel> texts = new List<TextModel>();

        using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:sql"]))
        {
            connection.Open();
            var command = new SqlCommand("select TextId, Title, CategoryName, WriteFullName, Number, Picture, PublishedInOlaganSiir,Text,Referances, Summary, CreatedTime,UpdatedTime from texts as te join Categories as ca on te.CategoryId = ca.CategoryId join Writers as wr on te.WriterId = wr.WriterId", connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                TextModel model = new TextModel();
                model.Id = reader.GetInt32(0);
                model.Title = reader.GetString(1);
                model.Category = reader.GetString(2);
                model.Writer = reader.GetString(3);
                model.picture = reader.GetString(5);
                model.Text = reader.GetString(7);
                model.Referances = reader.GetString(8);
                model.Summary = reader.GetString(9);
                texts.Add(model);
            }
        }
        return View(texts);
    }
    [Route("Admin/TextAdded")]

    public IActionResult TextAdded()
    {
        if (_loginControl.Control(HttpContext.Session.GetString("Giris")))
        {
            return RedirectToAction("login");
        }
        return View();
    }

    [Route("Admin/AddText")]
    public IActionResult AddText()
    {
        if (_loginControl.Control(HttpContext.Session.GetString("Giris")))
        {
            return RedirectToAction("login");
        }
        return View();
    }

    [Route("Admin/AddText")]

    [HttpPost]
    public IActionResult AddText(PostTextModel Model)
    {

        ViewBag.hata = null;

        using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:sql"]))
        {
            try
            {
                connection.Open();
                var command2 = new SqlCommand("select * from writers where WriteFullName = @Writer", connection);
                command2.Parameters.AddWithValue("@Writer", Model.Writer);
                var reader2 = command2.ExecuteReader();
                reader2.Read();
                int a = reader2.GetInt32(0);
                connection.Close();

                connection.Open();
                var command3 = new SqlCommand("select * from Categories where CategoryName = @CategoryName", connection);
                command3.Parameters.AddWithValue("@CategoryName", Model.Category);
                var reader3 = command3.ExecuteReader();
                reader3.Read();
                int b = reader3.GetInt32(0);
                connection.Close();

                connection.Open();
                var command = new SqlCommand("INSERT INTO Texts (Title, CategoryId, WriterId, Number, Picture, PublishedInOlaganSiir, [Text], Referances, Summary, CreatedTime,UpdatedTime) VALUES (@Title, @CategoryId, @WriterId, @Number, @Picture, @PublishedInOlaganSiir ,@Text, @Referances, @Summary, @CreatedTime, @UpdatedTime)", connection);
                command.Parameters.AddWithValue("@Title", Model.Title);
                command.Parameters.AddWithValue("@CategoryId", b);
                command.Parameters.AddWithValue("@WriterId", a);
                command.Parameters.AddWithValue("@Number", Model.number);
                command.Parameters.AddWithValue("@Picture", Model.picture);
                command.Parameters.AddWithValue("@PublishedInOlaganSiir", Model.PublishedInOlaganSiir);
                command.Parameters.AddWithValue("@Text", Model.Text);
                command.Parameters.AddWithValue("@Referances", Model.Referances);
                command.Parameters.AddWithValue("@Summary", Model.Summary);
                command.Parameters.AddWithValue("@CreatedTime", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedTime", DateTime.Now);

                command.ExecuteNonQuery();

                return RedirectToAction("TextAdded");
            }
            catch
            {

                ViewBag.hata = "İçerik Eklenemedi";
                return View();
            }
        }
    }

    [Route("Admin/TextDeleted")]

    public IActionResult TextDeleted()
    {
        if (_loginControl.Control(HttpContext.Session.GetString("Giris")))
        {
            return RedirectToAction("login");
        }
        return View();
    }
    [Route("Admin/DeleteText")]

    public IActionResult DeleteText(int a)
    {
        if (_loginControl.Control(HttpContext.Session.GetString("Giris")))
        {
            return RedirectToAction("login");
        }
        ViewBag.hata = null;
        try
        {
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:sql"]))
            {
                connection.Open();
                var command = new SqlCommand("delete from texts where TextId = @Id", connection);
                command.Parameters.AddWithValue("@Id", a);
                command.ExecuteNonQuery();
            }
            return RedirectToAction("TextDeleted");
        }
        catch (Exception)
        {

            ViewBag.hata = "Metin Silinemedi";
            return View();
        }
    }
    [Route("Admin/Login")]

    public IActionResult Login()
    {
        if (_loginControl.Control(HttpContext.Session.GetString("Giris")))
        {
            return View();
        }
        return RedirectToAction("Index");
    }

    [Route("Admin/Login")]
    [HttpPost]
    public IActionResult Login(LoginModel Model)
    {
        using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:sql"]))
        {

            connection.Open();
            var command = new SqlCommand("select * from Users where user_name= @user_name and password = @password", connection);
            command.Parameters.AddWithValue("@user_name", Model.UserName);
            command.Parameters.AddWithValue("@password", Model.Password);
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                HttpContext.Session.SetString("Giris", "Giriş Başarılı");
                return RedirectToAction("Index");
            }

            HttpContext.Session.SetString("Giris", "Kullanıcı Adı ya da Şifre Hatalı");

        }
        return View("login", HttpContext.Session.GetString("Giris"));
    }

    [Route("Admin/TextUpdated")]
    public IActionResult TextUpdated()
    {
        if (_loginControl.Control(HttpContext.Session.GetString("Giris")))
        {
            return RedirectToAction("login");
        }
        return View();
    }

    [Route("Admin/UpdateText")]
    public IActionResult UpdateText(int a)
    {
        if (_loginControl.Control(HttpContext.Session.GetString("Giris")))
        {
            return RedirectToAction("login");
        }

        TextModel model = new TextModel();
        using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:sql"]))
        {
            connection.Open();
            var command = new SqlCommand("select TextId, Title, CategoryName, WriteFullName, Number, Picture, PublishedInOlaganSiir,Text,Referances, Summary, CreatedTime,UpdatedTime from texts as te join Categories as ca on te.CategoryId = ca.CategoryId join Writers as wr on te.WriterId = wr.WriterId where TextId = @TextId", connection);
            command.Parameters.AddWithValue("@TextId", a);

            var reader = command.ExecuteReader();
            reader.Read();

            model.Id = reader.GetInt32(0);
            model.Title = reader.GetString(1);
            model.Category = reader.GetString(2);
            model.Writer = reader.GetString(3);
            model.number = reader.GetInt32(4).ToString();
            model.picture = reader.GetString(5);
            model.Text = reader.GetString(7);
            model.Referances = reader.GetString(8);
            model.Summary = reader.GetString(9);
        }
        return View(model);
    }

    [Route("Admin/UpdateText")]

    [HttpPost]
    public IActionResult UpdateText(TextModel Model)
    {
        ViewBag.hata = null;

        using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:sql"]))
        {
            try
            {
                connection.Open();
                var command2 = new SqlCommand("select * from writers where WriteFullName = @Writer", connection);
                command2.Parameters.AddWithValue("@Writer", Model.Writer);
                var reader2 = command2.ExecuteReader();
                reader2.Read();
                int Writer = reader2.GetInt32(0);
                connection.Close();

                connection.Open();
                var command3 = new SqlCommand("select * from Categories where CategoryName = @CategoryName", connection);
                command3.Parameters.AddWithValue("@CategoryName", Model.Category);
                var reader3 = command3.ExecuteReader();
                reader3.Read();
                int Category = reader3.GetInt32(0);
                connection.Close();

                connection.Open();

                var command = new SqlCommand("UPDATE Texts SET Title = @Title, CategoryId = @CategoryId, WriterId = @WriterId, Number = @Number, Picture = @Picture, PublishedInOlaganSiir = @PublishedInOlaganSiir, [Text]= @Text, Referances= @Referances, Summary= @Summary, CreatedTime=@CreatedTime,UpdatedTime= @UpdatedTime WHERE TextId = @ıd ", connection);

                command.Parameters.AddWithValue("@ıd", Model.Id);
                command.Parameters.AddWithValue("@Title", Model.Title);
                command.Parameters.AddWithValue("@CategoryId", Category);
                command.Parameters.AddWithValue("@WriterId", Writer);
                command.Parameters.AddWithValue("@Number", Convert.ToInt32(Model.number));
                command.Parameters.AddWithValue("@Picture", Model.picture);
                command.Parameters.AddWithValue("@PublishedInOlaganSiir", Model.PublishedInOlaganSiir);
                command.Parameters.AddWithValue("@Text", Model.Text);
                if (Model.Referances is not null)
                    command.Parameters.AddWithValue("@Referances", Model.Referances.ToString());
                command.Parameters.AddWithValue("@Summary", Model.Summary);
                command.Parameters.AddWithValue("@CreatedTime", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedTime", DateTime.Now);
                command.ExecuteNonQuery();

                return RedirectToAction("TextUpdated");
            }
            catch
            {
                ViewBag.hata = "İçerik Güncellenemedi";
                return View();
            }
        }
    }

    [Route("Admin/CommentControl")]

    public IActionResult CommentControl(bool Active = false)
    {
        if (_loginControl.Control(HttpContext.Session.GetString("Giris")))
        {
            return RedirectToAction("login");
        }

        ViewBag.Accept = Active;
        List<CommentModel> comments = new List<CommentModel>();

        using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:sql"]))
        {
            connection.Open();
            var commentCommand = new SqlCommand("select c.CommentId, c.Comment, c.TextId, c.Name, c.CommentDate,c.IsActive,c.Email,t.Title from comments AS c join texts as t on t.TextId = c.TextId where IsActive = @IsActive", connection);

            commentCommand.Parameters.AddWithValue("@IsActive", Active);

            var reader = commentCommand.ExecuteReader();

            while (reader.Read())
            {
                CommentModel comment = new();
                comment.CommentId = reader.GetInt32(0);
                comment.Comments = reader.GetString(1);
                comment.TextId = reader.GetInt32(2);
                comment.Name = reader.GetString(3);
                comment.CommentDate = reader.GetDateTime(4);
                comment.IsActive = reader.GetBoolean(5);
                comment.Email = reader.GetString(6);
                comment.Title = reader.GetString(7);
                comments.Add(comment);
            }

        }
        return View(comments);
    }
    [Route("Admin/AcceptComment")]

    public IActionResult AcceptComment(int a, bool delete = false)
    {
        if (_loginControl.Control(HttpContext.Session.GetString("Giris")))
        {
            return RedirectToAction("login");
        }
        if (delete)
        {
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:sql"]))
            {
                connection.Open();
                var command = new SqlCommand("delete from comments where CommentId = @Id", connection);
                command.Parameters.AddWithValue("@Id", a);
                command.ExecuteNonQuery();
            }
            RedirectToAction("CommentControl");

        }
        using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:sql"]))
        {
            connection.Open();
            var commentCommand = new SqlCommand("UPDATE comments SET IsActive = 1 WHERE CommentId = @ıd", connection);
            commentCommand.Parameters.AddWithValue("@ıd", a);
            commentCommand.ExecuteNonQuery();
        }
        return RedirectToAction("CommentControl");
    }

}
