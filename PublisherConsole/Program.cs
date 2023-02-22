using Microsoft.EntityFrameworkCore;
using PublisherData;
using PublisherDomain;
using System;

#pragma warning disable CS8321 // Local function is declared but never used
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.



using (PubContext context = new PubContext())
{
    context.Database.EnsureCreated();
}

// ExecutionTiming();
void ExecutionTiming()
{
    using var _context = new PubContext();
    //Nothing Happens
    var query = _context.Authors;
    //Now query executes
    List<Author> list = query.ToList();
    //Also here:
    foreach (var p in query) { }
    //And here:
    _ = query.FirstOrDefault();
    //And here:
    _ = query.SingleOrDefault(x => x.AuthorId == 1);
    //And here
    _ = _context.Authors.Find(1);
}


ParametrizedVariables();
void ParametrizedVariables()
{
    using var _context = new PubContext();

    var query = _context.Authors;

    _ = query.FirstOrDefault(x => x.AuthorId == 1);

    var id = 1;
    _ = query.FirstOrDefault(x => x.AuthorId == id);
    //And here
    _ = _context.Authors.Find(2);
}


// GetByPrimaryKey();
void GetByPrimaryKey()
{
    using var _context = new PubContext();
    //Get by primary key with immediate execution
    _ = _context.Authors.Find(1);
    //Complex PK with immediate execution
    _ = _context.Artists.Find(2, 1688);
}

// GetSingleRecord();
void GetSingleRecord()
{
    using var _context = new PubContext();

    //All immediate execution
    //NOTE: should use an order by with these
    _ = _context.Authors.Where(x => x.AuthorId == 1).FirstOrDefault();
    _ = _context.Authors.FirstOrDefault(x => x.AuthorId == 1);
    //Using Single - Exception if more than one is found
    _ = _context.Authors.SingleOrDefault(x => x.AuthorId == 1);
}

// SelectWithMultipleClauses();
void SelectWithMultipleClauses()
{
    using var _context = new PubContext();
    //All in one statement
    var query1 = _context.Authors
        .Where(x => x.FirstName == "em" && x.AuthorId == 1);
    //Chained statements
    var query2 = _context.Authors
        .Where(x => x.FirstName == "em").Where(x => x.AuthorId == 1);
    //Built up over disparate calls
    var query3 = _context.Authors.Where(x => x.FirstName == "em");
    query3 = query3.Where(x => x.AuthorId == 1);

    //Or's can't be chained
    var query4 = _context.Authors
        .Where(x => x.FirstName == "em" || x.AuthorId == 1);
}

// UsingCSharpLikeFunction();
void UsingCSharpLikeFunction()
{
    using var _context = new PubContext();
    List<int> list = new List<int> { 1, 3, 5 };
    
    var query = _context.Authors.Where(x => list.Contains(x.AuthorId));
    _ = _context.Authors.Where(x => x.LastName.Contains("UF"));
    _ = _context.Authors.Where(x => EF.Functions.Like(x.LastName, "%UF%"));
    //IsDate translates to the TSQL IsDate function 
    _ = _context.Authors.Where(x => EF.Functions.IsDate(x.FirstName));
    decimal sum = _context.Books.Sum(x => x.BasePrice);
    int count = _context.Books.Count(x => x.BasePrice != 0);
    decimal avg = _context.Books.Average(x => (decimal?)x.BasePrice) ?? 0;
    decimal max = _context.Books.Max(x => (decimal?)x.BasePrice) ?? 0;
    decimal min = _context.Books.Min(x => (decimal?)x.BasePrice) ?? 0;
    bool any = _context.Books.Any(x => x.BasePrice != 0);
    bool all = _context.Books.All(x => x.BasePrice != 0);
}

// SortDataServerSide();
void SortDataServerSide()
{
    using var _context = new PubContext();

    IOrderedQueryable<Author> query1 =
        _context.Authors.OrderBy(x => x.AuthorId).ThenBy(x => x.FirstName);
    IOrderedQueryable<Author> query2 =
        _context.Authors.OrderByDescending(x => x.AuthorId).ThenBy(x => x.FirstName);
}

// PageRecords();
void PageRecords()
{
    using var _context = new PubContext();
    var prodList = _context.Authors
        .Where(x => x.FirstName == "em")
        .OrderBy(x => x.AuthorId)
        .Skip(25).Take(50);
}

//AddAuthorWithBook();
void AddAuthorWithBook()
{
    var author = new Author { FirstName = "Julie", LastName = "Lerman" };
    author.Books.Add(new Book
    {
        Title = "Programming Entity Framework",
        PublishDate = new DateTime(2009, 1, 1)
    });
    author.Books.Add(new Book
    {
        Title = "Programming Entity Framework 2nd Ed",
        PublishDate = new DateTime(2010, 8, 1)
    });
    using var context = new PubContext();
    context.Authors.Add(author);
    context.SaveChanges();
}

// GetAuthorsWithBooks();
void GetAuthorsWithBooks()
{
    using var context = new PubContext();
    var authors = context.Authors.Include(a => a.Books).ToList();
    foreach (var author in authors)
    {
        Console.WriteLine(author.FirstName + " " + author.LastName);
        foreach (var book in author.Books)
        {
            Console.WriteLine(book.Title);
        }
    }
}

// AddAuthor();
void AddAuthor()
{
    var author = new Author { FirstName = "Josie", LastName = "Newf" };
    using var context = new PubContext();
    context.Authors.Add(author);
    context.SaveChanges();
}

// GetAuthors();
void GetAuthors()
{
    using var context = new PubContext();
    var authors = context.Authors.ToList();
    foreach (var author in authors)
    {
        Console.WriteLine(author.FirstName + " " + author.LastName);
    }
}


PubContext _context = new PubContext();
//this assumes you are working with the populated
//database created in previous module

// QueryFilters();
void QueryFilters()
{
    //var name = "Josie";
    //var authors=_context.Authors.Where(s=>s.FirstName==name).ToList();
    var filter = "L%";
    var authors = _context.Authors
        .Where(a => EF.Functions.Like(a.LastName, filter)).ToList();
}

// QueryAggregate();
void QueryAggregate()
{
    var author = _context.Authors.OrderByDescending(a => a.FirstName)
        .FirstOrDefault(a => a.LastName == "Lerman");
}

// SortAuthors();
void SortAuthors()
{
    var authorsByLastName = _context.Authors
        .OrderBy(a => a.LastName)
        .ThenBy(a => a.FirstName).ToList();
    authorsByLastName.ForEach(a => Console.WriteLine(a.LastName + "," + a.FirstName));

    var authorsDescending = _context.Authors
        .OrderByDescending(a => a.LastName)
        .ThenByDescending(a => a.FirstName).ToList();
    Console.WriteLine("**Descending Last and First**");
    authorsDescending.ForEach(a => Console.WriteLine(a.LastName + "," + a.FirstName));
    var lermans = _context.Authors.Where(a => a.LastName == "Lerman").OrderByDescending(a => a.FirstName).ToList();
}

// FindIt();
void FindIt()
{
    var authorIdTwo = _context.Authors.Find(2);
}

// AddSomeMoreAuthors();
void AddSomeMoreAuthors()
{
    _context.Authors.Add(new Author { FirstName = "Rhoda", LastName = "Lerman" });
    _context.Authors.Add(new Author { FirstName = "Don", LastName = "Jones" });
    _context.Authors.Add(new Author { FirstName = "Jim", LastName = "Christopher" });
    _context.Authors.Add(new Author { FirstName = "Stephen", LastName = "Haunts" });
    _context.SaveChanges();
}

// SkipAndTakeAuthors();
void SkipAndTakeAuthors()
{
    var groupSize = 2;
    for (int i = 0; i < 5; i++)
    {
        var authors = _context.Authors.Skip(groupSize * i).Take(groupSize).ToList();
        Console.WriteLine($"Group {i}:");
        foreach (var author in authors)
        {
            Console.WriteLine($" {author.FirstName} {author.LastName}");
        }
    }
}


// InsertMultipleAuthors();
void InsertMultipleAuthors()
{
    var newAuthors = new Author[]{
       new Author { FirstName = "Ruth", LastName = "Ozeki" },
       new Author { FirstName = "Sofia", LastName = "Segovia" },
       new Author { FirstName = "Ursula K.", LastName = "LeGuin" },
       new Author { FirstName = "Hugh", LastName = "Howey" },
       new Author { FirstName = "Isabelle", LastName = "Allende" }
    };
    _context.AddRange(newAuthors);
    _context.SaveChanges();
}

// DeleteAnAuthor1();
void DeleteAnAuthor1()
{
    var extraJL = _context.Authors.Find(1);
    if (extraJL != null)
    {
        _context.Authors.Remove(extraJL);
        _context.SaveChanges();
    }
}

// VariousOperations();
void VariousOperations()
{
    var author = _context.Authors.Find(2); //this is currently Josie Newf
    author.LastName = "Newfoundland";
    var newauthor = new Author { LastName = "Appleman", FirstName = "Dan" };
    _context.Authors.Add(newauthor);
    _context.SaveChanges();
}

// RetrieveAndUpdateAuthor();
void RetrieveAndUpdateAuthor()
{
    var author = _context.Authors.FirstOrDefault(a => a.FirstName == "Julie" && a.LastName == "Lerman");
    if (author != null)
    {
        author.FirstName = "Julia";
        _context.SaveChanges();
    }
}

// CoordinatedRetrieveAndUpdateAuthor();
void CoordinatedRetrieveAndUpdateAuthor()
{
    var author = FindThatAuthor(3);
    if (author?.FirstName == "Julie")
    {
        author.FirstName = "Julia";
        SaveThatAuthor(author);
    }
}

// FindThatAuthor();
Author FindThatAuthor(int authorId)
{
    using var shortLivedContext = new PubContext();

    return shortLivedContext.Authors.Find(authorId);

}

// SaveThatAuthor();
void SaveThatAuthor(Author author)
{
    using var anotherShortLivedContext = new PubContext();
    anotherShortLivedContext.Authors.Update(author);
    anotherShortLivedContext.SaveChanges();
}


// RetrieveAndUpdateMultipleAuthors();
void RetrieveAndUpdateMultipleAuthors()
{
    var LermanAuthors = _context.Authors.Where(a => a.LastName == "Lerman").ToList();
    //foreach (var la in LermanAuthors)
    //{
    //    la.LastName = "Lermann";
    //}
    var a1 = LermanAuthors[0];
    var a2 = LermanAuthors[1];
    a1 = null;
    Console.WriteLine("Before" + _context.ChangeTracker.DebugView.ShortView);

    //_context.ChangeTracker.DetectChanges();
    //Console.WriteLine("After:" + _context.ChangeTracker.DebugView.ShortView);
    // LermanAuthors.RemoveAt(0);
    _context.ChangeTracker.DetectChanges();
    // _context.SaveChanges();
    Console.WriteLine("After:" + _context.ChangeTracker.DebugView.ShortView);
}

// InsertAuthor();
void InsertAuthor()
{
    var author = new Author { FirstName = "Frank", LastName = "Herbert" };
    _context.Authors.Add(author);
    _context.SaveChanges();
}

// InsertMultipleAuthorsPassedIn();
void InsertMultipleAuthorsPassedIn(List<Author> listOfAuthors)
{
    _context.Authors.AddRange(listOfAuthors);
    _context.SaveChanges();
}

// BulkAddUpdate();
void BulkAddUpdate()
{
    var newAuthors = new Author[]{
     new Author { FirstName = "Tsitsi", LastName = "Dangarembga" },
     new Author { FirstName = "Lisa", LastName = "See" },
     new Author { FirstName = "Zhang", LastName = "Ling" },
     new Author { FirstName = "Marilynne", LastName="Robinson"}
    };
    _context.Authors.AddRange(newAuthors);
    var book = _context.Books.Find(2);
    book.Title = "Programming Entity Framework 2nd Edition";
    _context.SaveChanges();
}


//_context.Database.EnsureDeleted();
//_context.Database.EnsureCreated();

// AddBook();
void AddBook()
{
    var book = new Book { Title = "How to crash your app" };
    _context.Books.Add(book);
    _context.SaveChanges();
}

// DeleteAnAuthor();
void DeleteAnAuthor()
{
    var author = _context.Authors.Find(2);
    _context.Authors.Remove(author);
    _context.SaveChanges();
}

//CascadeDeleteInActionWhenTracked();

void CascadeDeleteInActionWhenTracked()
{
    //note : I knew that author with id 2 had books in my sample database
    var author = _context.Authors.Include(a => a.Books)
     .FirstOrDefault(a => a.AuthorId == 2);
    author.Books.Remove(author.Books[0]);
    _context.ChangeTracker.DetectChanges();
    var state = _context.ChangeTracker.DebugView.ShortView;
    //_context.SaveChanges();
}


//ModifyingRelatedDataWhenTracked();

void ModifyingRelatedDataWhenTracked()
{
    var author = _context.Authors.Include(a => a.Books)
        .FirstOrDefault(a => a.AuthorId == 5);
    //author.Books[0].BasePrice = (decimal)10.00;
    author.Books.Remove(author.Books[1]);
    _context.ChangeTracker.DetectChanges();
    var state = _context.ChangeTracker.DebugView.ShortView;

}


//RemovingRelatedData();
void RemovingRelatedData()
{
    var author = _context.Authors.Include(a => a.Books)
        .FirstOrDefault(a => a.AuthorId == 5);
    var book = author.Books[0];
    author.Books.Remove(book);
    // _context.Books.Remove(author.Books[1]);
    _context.ChangeTracker.DetectChanges();
    var state = _context.ChangeTracker.DebugView.ShortView;

}


//ModifyingRelatedDataWhenNotTracked();

void ModifyingRelatedDataWhenNotTracked()
{
    var author = _context.Authors.Include(a => a.Books)
        .FirstOrDefault(a => a.AuthorId == 5);
    author.Books[0].BasePrice = (decimal)12.00;

    var newContext = new PubContext();
    //newContext.Books.Update(author.Books[0]);
    newContext.Entry(author.Books[0]).State = EntityState.Modified;
    var state = newContext.ChangeTracker.DebugView.ShortView;
    newContext.SaveChanges();
}


//FilterUsingRelatedData();
void FilterUsingRelatedData()
{
    var recentAuthors = _context.Authors
        .Where(a => a.Books.Any(b => b.PublishDate.Year >= 2015))
        .ToList();
}

// LazyLoadBooksFromAnAuthor();
void LazyLoadBooksFromAnAuthor()
{
    //requires lazy loading to be set up in your app
    var author = _context.Authors.FirstOrDefault(a => a.LastName == "Howey");
    foreach (var book in author.Books)
    {
        Console.WriteLine(book.Title);
    }
}

//Projections();
void Projections()
{
    var unknownTypes = _context.Authors
        .Select(a => new
        {
            AuthorId = a.AuthorId,
            Name = a.FirstName.First() + "" + a.LastName,
            Books = a.Books   //.Where(b => b.PublishDate.Year < 2000).Count()
        })
        .ToList();
    var debugview = _context.ChangeTracker.DebugView.ShortView;
}


//ExplicitLoadCollection();
void ExplicitLoadCollection()
{
    var author = _context.Authors.FirstOrDefault(a => a.LastName == "Howey");
    _context.Entry(author).Collection(a => a.Books).Load();

}

//ExplicitLoadReference();
void ExplicitLoadReference()
{
    var book = _context.Books.FirstOrDefault();
    _context.Entry(book).Reference(b => b.Author).Load();
}


//EagerLoadBooksWithAuthors();
void EagerLoadBooksWithAuthors()
{
    //var authors = _context.Authors.Include(a => a.Books).ToList();
    var pubDateStart = new DateTime(2010, 1, 1);
    var authors = _context.Authors
        .Include(a => a.Books
                       .Where(b => b.PublishDate >= pubDateStart)
                       .OrderBy(b => b.Title))
        .ToList();

    authors.ForEach(a =>
    {
        Console.WriteLine($"{a.LastName} ({a.Books.Count})");
        a.Books.ForEach(b => Console.WriteLine("     " + b.Title));
    });
}

// EagerLoadBooksWithAuthorsVariations();
void EagerLoadBooksWithAuthorsVariations()
{
    var lAuthors = _context.Authors.Where(a => a.LastName.StartsWith("L"))
        .Include(a => a.Books).ToList();
    var lerman = _context.Authors.Where(a => a.LastName == "Lerman")
        .Include(a => a.Books).FirstOrDefault();
}

//InsertNewAuthorWithNewBook();
void InsertNewAuthorWithNewBook()
{
    var author = new Author { FirstName = "Lynda", LastName = "Rutledge" };
    author.Books.Add(new Book
    {
        Title = "West With Giraffes",
        PublishDate = new DateTime(2021, 2, 1)
    });
    _context.Authors.Add(author);
    _context.SaveChanges();
}

//InsertNewAuthorWith2NewBooks();
void InsertNewAuthorWith2NewBooks()
{
    var author = new Author { FirstName = "Don", LastName = "Jones" };
    author.Books.AddRange(new List<Book> {
        new Book {Title = "The Never", PublishDate = new DateTime(2019, 12, 1) },
        new Book {Title = "Alabaster", PublishDate = new DateTime(2019,4,1)}
    });
    _context.Authors.Add(author);
    _context.SaveChanges();
}

//AddNewBookToExistingAuthorInMemory();
void AddNewBookToExistingAuthorInMemory()
{
    var author = _context.Authors.FirstOrDefault(a => a.LastName == "Howey");
    if (author != null)
    {
        author.Books.Add(
          new Book { Title = "Wool", PublishDate = new DateTime(2012, 1, 1) }
          );
    }
    _context.SaveChanges();
}

//AddNewBookToExistingAuthorInMemoryViaBook();
void AddNewBookToExistingAuthorInMemoryViaBook()
{
    var book = new Book
    {
        Title = "Shift",
        PublishDate = new DateTime(2012, 1, 1),
        AuthorId = 5
    };
    // book.Author = _context.Authors.Find(5); //known id for Hugh Howey
    _context.Books.Add(book);
    _context.SaveChanges();
}

//UnAssignAnArtistFromACover();
void UnAssignAnArtistFromACover()
{
    var coverwithartist = _context.Covers
        .Include(c => c.Artists.Where(a => a.ArtistId == 2))
        .FirstOrDefault(c => c.CoverId == 1);
    //coverwithartist.Artists.RemoveAt(0);
    _context.Artists.Remove(coverwithartist.Artists[0]);
    _context.ChangeTracker.DetectChanges();
    var debugview = _context.ChangeTracker.DebugView.ShortView;
    //_context.SaveChanges();
}

//DeleteAnObjectThatsInARelationship();

void DeleteAnObjectThatsInARelationship()
{
    var cover = _context.Covers.Find(4);
    _context.Covers.Remove(cover);
    _context.SaveChanges();
}

//ReassignACover();

void ReassignACover()
{
    var coverwithartist4 = _context.Covers
    .Include(c => c.Artists.Where(a => a.ArtistId == 4))
    .FirstOrDefault(c => c.CoverId == 5);

    coverwithartist4.Artists.RemoveAt(0);

    var artist3 = _context.Artists.Find(3);
    coverwithartist4.Artists.Add(artist3);
    _context.ChangeTracker.DetectChanges();


}

//RetrieveAnArtistWithTheirCovers();
void RetrieveAnArtistWithTheirCovers()
{
    var artistWithCovers = _context.Artists.Include(a => a.Covers)
                            .FirstOrDefault(a => a.ArtistId == 1);
}

//RetrieveACoverWithItsArtists();
void RetrieveACoverWithItsArtists()
{
    var coverWithArtists = _context.Covers.Include(c => c.Artists)
                            .FirstOrDefault(c => c.CoverId == 1);
}

//RetrieveAllArtistsWithTheirCovers();
void RetrieveAllArtistsWithTheirCovers()
{
    var artistsWithCovers = _context.Artists.Include(a => a.Covers).ToList();

    foreach (var a in artistsWithCovers)
    {
        Console.WriteLine($"{a.FirstName} {a.LastName}, Designs to work on:");
        var primaryArtistId = a.ArtistId;
        if (a.Covers.Count() == 0)
        {
            Console.WriteLine("  No covers");
        }
        else
        {
            foreach (var c in a.Covers)
            {
                string collaborators = "";
                foreach (var ca in c.Artists.Where(ca => ca.ArtistId != primaryArtistId))
                {
                    collaborators += $"{ca.FirstName} {ca.LastName}";
                }
                if (collaborators.Length > 0)
                { collaborators = $"(with {collaborators})"; }
                Console.WriteLine($"  *{c.DesignIdeas} {collaborators}");
            }
        }
    }
}


//RetrieveAllArtistsWhoHaveCovers();

void RetrieveAllArtistsWhoHaveCovers()
{
    var artistsWithCovers = _context.Artists.Where(a => a.Covers.Any()).ToList();
}

//ConnectExistingArtistAndCoverObjects();
void ConnectExistingArtistAndCoverObjects()
{
    var artistA = _context.Artists.Find(1);
    var artistB = _context.Artists.Find(2);
    var coverA = _context.Covers.Find(1);
    coverA.Artists.Add(artistA);
    coverA.Artists.Add(artistB);
    _context.SaveChanges();
}

// CreateNewCoverWithExistingArtist();
void CreateNewCoverWithExistingArtist()
{
    var artistA = _context.Artists.Find(1);
    var cover = new Cover { DesignIdeas = "Author has provided a photo" };
    artistA.Covers.Add(cover);
    //cover.Artists.Add(artistA);
    //_context.Covers.Add(cover);
    _context.SaveChanges();
}

//CreateNewCoverAndArtistTogether();

void CreateNewCoverAndArtistTogether()
{
    var newArtist = new Artist { FirstName = "Kir", LastName = "Talmage" };
    var newCover = new Cover { DesignIdeas = "We like birds!" };
    newArtist.Covers.Add(newCover);
    _context.Artists.Add(newArtist);
    _context.SaveChanges();
}

//GetAllBooksWithTheirCovers();
void GetAllBooksWithTheirCovers()
{
    var booksandcovers = _context.Books.Include(b => b.Cover).ToList();
    booksandcovers.ForEach(book =>
     Console.WriteLine(
         book.Title +
         (book.Cover == null ? ": No cover yet" : ":" + book.Cover.DesignIdeas)));
}
//GetAllBooksThatHaveCovers();
void GetAllBooksThatHaveCovers()
{
    var booksandcovers = _context.Books.Include(b => b.Cover).Where(b => b.Cover != null).ToList();
    booksandcovers.ForEach(book =>
       Console.WriteLine(book.Title + ":" + book.Cover.DesignIdeas));
}

//ProjectBooksThatHaveCovers();
void ProjectBooksThatHaveCovers()
{
    var anon = _context.Books.Where(b => b.Cover != null)
      .Select(b => new { b.Title, b.Cover.DesignIdeas })
      .ToList();
    anon.ForEach(b =>
      Console.WriteLine(b.Title + ": " + b.DesignIdeas));

}

// MultiLevelInclude();
void MultiLevelInclude()
{
    var authorGraph = _context.Authors.AsNoTracking()
        .Include(a => a.Books)
        .ThenInclude(b => b.Cover)
        .ThenInclude(c => c.Artists)
        .FirstOrDefault(a => a.AuthorId == 1);

    Console.WriteLine(authorGraph?.FirstName + " " + authorGraph?.LastName);
    foreach (var book in authorGraph.Books)
    {
        Console.WriteLine("Book:" + book.Title);
        if (book.Cover != null)
        {
            Console.WriteLine("Design Ideas: " + book.Cover.DesignIdeas);
            Console.Write("Artist(s):");
            book.Cover.Artists.ForEach(a => Console.Write(a.LastName + " "));

        }
    }
};



//NewBookAndCover();
void NewBookAndCover()
{
    var book = new Book
    {
        AuthorId = 1,
        Title = "Call Me Ishtar",
        PublishDate = new DateTime(1973, 1, 1)
    };
    book.Cover = new Cover { DesignIdeas = "Image of Ishtar?" };
    _context.Books.Add(book);
    _context.SaveChanges();
}

//AddCoverToExistingBook();
void AddCoverToExistingBook()
{
    var book = _context.Books.Find(7); //Wool
    book.Cover = new Cover { DesignIdeas = "A wool scouring pad" };
    _context.SaveChanges();
}


//AddCoverToExistingBookThatHasAnUnTrackedCover();
void AddCoverToExistingBookThatHasAnUnTrackedCover()
{
    var book = _context.Books.Find(5); //The Never
    book.Cover = new Cover { DesignIdeas = "A spiral" };
    _context.SaveChanges();
}

//AddCoverToExistingBookWithTrackedCover();
void AddCoverToExistingBookWithTrackedCover()
{
    var book = _context.Books.Include(b => b.Cover)
                             .FirstOrDefault(b => b.BookId == 5); //The Never
    book.Cover = new Cover { DesignIdeas = "A spiral" };
    _context.ChangeTracker.DetectChanges();
    var debugview = _context.ChangeTracker.DebugView.ShortView;
}

//ProtectingFromUniqueFK();
void ProtectingFromUniqueFK()
{
    var TheNeverDesignIdeas = "A spirally spiral";
    var book = _context.Books.Include(b => b.Cover)
                             .FirstOrDefault(b => b.BookId == 5); //The Never
    if (book.Cover != null)
    {
        book.Cover.DesignIdeas = TheNeverDesignIdeas;
    }
    else
    {
        book.Cover = new Cover { DesignIdeas = "A spirally spiral" };
    }
    _context.SaveChanges();
}


//MoveCoverFromOneBookToAnother();
void MoveCoverFromOneBookToAnother()
{
    ///"we like birds"coverid 5, currenlty assigned to The Never bookid 5
    var cover = _context.Covers.Include(c => c.Book).FirstOrDefault(c => c.CoverId == 5);
    var newBook = _context.Books.Find(3);
    cover.Book = newBook;
    _context.ChangeTracker.DetectChanges();
    var debugview = _context.ChangeTracker.DebugView.ShortView;
}

// DeleteCoverFromBook();

void DeleteCoverFromBook()
{
    var book = _context.Books.Include(b => b.Cover).FirstOrDefault(b => b.BookId == 5);
    book.Cover = null;
    _context.ChangeTracker.DetectChanges();
    var debugview = _context.ChangeTracker.DebugView.ShortView;
}

// DeleteCover(10);

void DeleteCover(int coverId)
{
    var rowCount = _context.Database.ExecuteSqlRaw("DeleteCover {0}", coverId);
    Console.WriteLine(rowCount);
}

// GetAuthorsByArtist();

void GetAuthorsByArtist()
{
    var authorartists = _context.AuthorsByArtist.ToList();
    var oneauthorartists = _context.AuthorsByArtist.FirstOrDefault();
    var Kauthorartists = _context.AuthorsByArtist
                                 .Where(a => a.Artist.StartsWith("K")).ToList();
    var debugView = _context.ChangeTracker.DebugView.ShortView;
}



// RawSqlStoredProc();
void RawSqlStoredProc()
{
    var authors = _context.Authors
        .FromSqlRaw("AuthorsPublishedinYearRange {0}, {1}", 2010, 2015)
        .ToList();
}

// InterpolatedSqlStoredProc();
void InterpolatedSqlStoredProc()
{
    int start = 2010;
    int end = 2015;
    var authors = _context.Authors
    .FromSqlInterpolated($"AuthorsPublishedinYearRange {start}, {end}")
    .ToList();
}


//SimpleRawSQL();
void SimpleRawSQL()
{
    var authors = _context.Authors.FromSqlRaw("select * from authors").OrderBy(a => a.LastName).ToList();
}

//ConcatenatedRawSql_Unsafe(); //There is no safe way with concatentation!
void ConcatenatedRawSql_Unsafe()
{
    var lastnameStart = "L";
    var authors = _context.Authors
        .FromSqlRaw("SELECT * FROM authors WHERE lastname LIKE '" + lastnameStart + "%'")
        .OrderBy(a => a.LastName).TagWith("Concatenated_Unsafe").ToList();
}

//FormattedRawSql_Unsafe();
void FormattedRawSql_Unsafe()
{
    var lastnameStart = "L";
    var sql = String.Format("SELECT * FROM authors WHERE lastname LIKE '{0}%'", lastnameStart);
    var authors = _context.Authors.FromSqlRaw(sql)
        .OrderBy(a => a.LastName).TagWith("Formatted_Unsafe").ToList();
}

//FormattedRawSql_Safe();
void FormattedRawSql_Safe()
{
    var lastnameStart = "L";
    var authors = _context.Authors
        .FromSqlRaw("SELECT * FROM authors WHERE lastname LIKE '{0}%'", lastnameStart)
        .OrderBy(a => a.LastName).TagWith("Formatted_Safe").ToList();
}

//StringFromInterpolated_Unsafe();
void StringFromInterpolated_Unsafe()
{
    var lastnameStart = "L";
    string sql = $"SELECT * FROM authors WHERE lastname LIKE '{lastnameStart}%'";
    var authors = _context.Authors.FromSqlRaw(sql)
        .OrderBy(a => a.LastName).TagWith("Interpolated_Unsafe").ToList();
}

//StringFromInterpolated_StillUnsafe();
void StringFromInterpolated_StillUnsafe()
{
    var lastnameStart = "L";
    var authors = _context.Authors
        .FromSqlRaw($"SELECT * FROM authors WHERE lastname LIKE '{lastnameStart}%'")
        .OrderBy(a => a.LastName).TagWith("Interpolated_StillUnsafe").ToList();
}

//StringFromInterpolated_Safe();
void StringFromInterpolated_Safe()
{
    var lastnameStart = "L";
    var authors = _context.Authors
        .FromSqlInterpolated($"SELECT * FROM authors WHERE lastname LIKE '{lastnameStart}%'")
    .OrderBy(a => a.LastName).TagWith("Interpolated_Safe").ToList();
}

#if false
StringFromInterpolated_SoSafeItWontCompile();
void StringFromInterpolated_SoSafeItWontCompile()
{
    var lastnameStart = "L";
    var sql = $"SELECT * FROM authors WHERE lastname LIKE '{lastnameStart}%'";
    var authors = _context.Authors.FromSqlInterpolated(sql)
    .OrderBy(a => a.LastName).TagWith("Interpolated_WontCompile").ToList();
}

FormattedWithInterpolated_SoSafeItWontCompile();
void FormattedWithInterpolated_SoSafeItWontCompile()
{
    var lastnameStart = "L";
    var authors = _context.Authors
        .FromSqlInterpolated
            ("SELECT * FROM authors WHERE lastname LIKE '{0}%'", lastnameStart)
        .OrderBy(a => a.LastName).TagWith("Interpolated_WontCompile").ToList();
}
#endif
