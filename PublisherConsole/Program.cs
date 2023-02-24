using Microsoft.EntityFrameworkCore;
using PublisherData;
using PublisherDomain;

#pragma warning disable CS8321 // Local function is declared but never used
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.


using (PubContext context = new PubContext())
{
    context.Database.EnsureCreated();
}

PubContext _context = new PubContext();

 //ExecutionTiming();
void ExecutionTiming()
{
    //Nothing Happens
    var authorsDbSet = _context.Authors;
    //Now authorsDbSet executes
    List<Author> list = authorsDbSet.ToList();

    // These methods trigger immediate executions of the queries
    // ToList()
    // First()
    // FirstOrDefault()
    // Single()
    // SingleOrDefault()
    // Find()
    // SaveChanges()
    // ExecuteUpdate()
    // ExecuteDelete()

    // ToListAsync()
    // FirstAsync()
    // FirstOrDefaultAsync()
    // SingleAsync()
    // SingleOrDefaultAsync()
    // FindAsync()
    // SaveChangesAsync()
    // ExecuteUpdateAsync()
    // ExecuteDeleteAsync()
    // etc....

    //Also here:
    foreach (var p in authorsDbSet) {
        Console.WriteLine(p.FirstName);
    }
    //And here:
    _ = authorsDbSet.FirstOrDefault();
    //And here:
    _ = authorsDbSet.SingleOrDefault(x => x.AuthorId == 1);
}


// ParametrizedVariables();
void ParametrizedVariables()
{
    var query = _context.Authors;

    _ = query.FirstOrDefault(x => x.AuthorId == 1);

    var id = 1;
    _ = query.FirstOrDefault(x => x.AuthorId == id);

    // GetByPrimaryKey
    _ = _context.Authors.Find(2);
    _ = _context.Authors.Find(2, 1688);
}

// SelectWithMultipleClauses();
void SelectWithMultipleClauses()
{
    //Chained statements
    var query1 = _context.Authors
        .Where(x => x.FirstName.Contains("a"))
        .OrderBy(x => x.FirstName);

    _ = query1.ToList();

    var query2 = _context.Authors
        .Where(x => x.FirstName.Contains("a"));

    query2 = query2
        .OrderBy(x => x.FirstName);

    _ = query2.ToList();
}

// UsingCSharpLikeFunction();
void UsingCSharpLikeFunction()
{
    // All LINQ methods are available in EF

    decimal sum = _context.Books.Sum(x => x.BasePrice);
    int count = _context.Books.Count(x => x.BasePrice != 0);
    decimal avg = _context.Books.Average(x => (decimal?)x.BasePrice) ?? 0;
    decimal max = _context.Books.Max(x => (decimal?)x.BasePrice) ?? 0;
    decimal min = _context.Books.Min(x => (decimal?)x.BasePrice) ?? 0;
    bool any = _context.Books.Any(x => x.BasePrice != 0);
    bool all = _context.Books.All(x => x.BasePrice == 0);
    var query1 = _context.Authors
        .OrderBy(x => x.AuthorId)
        .ThenBy(x => x.FirstName);
    var  query2 = _context.Authors
        .OrderByDescending(x => x.AuthorId)
        .ThenBy(x => x.FirstName);
}

// PageRecords();
void PageRecords()
{
    var prodList = _context.Authors
        .Where(x => x.FirstName != "aaa")
        .OrderBy(x => x.AuthorId)
        .Skip(2)
        .Take(5)
        .ToList();
}

// AddAuthorWithBook();
void AddAuthorWithBook()
{
    var author1 = new Author { FirstName = "Julie", LastName = "Lerman" };
    author1.Books.Add(new Book
    {
        Title = "Programming Entity Framework",
        PublishDate = new DateTime(2009, 1, 1)
    });
    author1.Books.Add(new Book
    {
        Title = "Programming Entity Framework 2nd Ed",
        PublishDate = new DateTime(2010, 8, 1)
    });

    var author2 = new Author { FirstName = "Mehmet", LastName = "Inan" };

    // 2 ways to add object
    _context.Add(author2);
    _context.Authors.Add(author1);
    
    // EF will set Book Id and Author ID and will update the objects after authorsDbSet execution
    _context.SaveChanges();

    author1.FirstName = "New author1";
    author1.Books.RemoveAt(0);

    author2.LastName = "INAN";

    _context.SaveChanges();
}

// ChangeTracker();
void ChangeTracker()
{
    var author1 = new Author { FirstName = "Julie", LastName = "Lerman" };
    author1.Books.Add(new Book
    {
        Title = "Programming Entity Framework",
        PublishDate = new DateTime(2009, 1, 1)
    });
    author1.Books.Add(new Book
    {
        Title = "Programming Entity Framework 2nd Ed",
        PublishDate = new DateTime(2010, 8, 1)
    });

    var author2 = new Author { FirstName = "Mehmet", LastName = "Inan" };

    // EF creates insert/update/delete queries based on the tracking that is done by the ChangeTracker

    // The changes detected by the ChangeTracker can be seen in here: _context.ChangeTracker.DebugView.ShortView

    _ = _context.ChangeTracker.DebugView.ShortView;

    // Objects that are retrieved through context are tracked.

    _context.Add(author2);
    _context.Authors.Add(author1);

    // If you call SaveChanges(), EF will call ChangeTracker.DetectChanges and save changes based on the results in the ChangeTracker.
    // However, you can manually call the method.
    // DetextChanges causes the ChangeTracker to update its knowledge of the state of all objects that it is tracking.

    _ = _context.ChangeTracker.DebugView.ShortView;

    // Will trigger authorsDbSet execution
    // EF will set Book Id and Author ID and will update the objects after authorsDbSet execution
    _context.SaveChanges();

    author1.FirstName = "New author1";
    author1.Books.RemoveAt(0);

    _context.ChangeTracker.DetectChanges();

    _ = _context.ChangeTracker.DebugView.ShortView;
    _ = _context.ChangeTracker.DebugView.LongView;

    _context.SaveChanges();

    _ = _context.ChangeTracker.DebugView.ShortView;
}

// NoTracking();
void NoTracking()
{
    ///// AsNoTracking()
    var authors = _context.Authors
        // .AsNoTracking()
        .ToList();

    var books = _context.Books
        .AsNoTracking() // *********
        .ToList();

    var debugview = _context.ChangeTracker.DebugView.ShortView;
};

/*
 * Ways To Get data from the DB
 * - Eager Loading
 * - Query Projections
 * - Explicit Loading
 * - Lazy Loading
 */

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
}

// EagerLoadBooksWithAuthorsVariations();
void EagerLoadBooksWithAuthorsVariations()
{
    var lAuthors = _context.Authors
        .Where(a => a.LastName.StartsWith("L"))
        .Include(a => a.Books).ToList();

    // Include can be added before or after Where statement
    var lAuthors1 = _context.Authors
        .Include(a => a.Books)
        .Where(a => a.LastName.StartsWith("L"))
        .ToList();
}


 // MultiLevelInclude();
void MultiLevelInclude()
{
    var authorGraph = _context.Authors
        .Include(a => a.Books)
            .ThenInclude(b => b.Cover)
            .ThenInclude(c => c.Artists)
        .FirstOrDefault(a => a.AuthorId == 1);
    var debugview = _context.ChangeTracker.DebugView.ShortView;
};


// Projections();
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


// LazyLoadBooksFromAnAuthor();
void LazyLoadBooksFromAnAuthor()
{
    // Lazy loading is disabled by default
    // Requires additional configuration to enable Lazy loading
    var authors = _context.Authors.ToList();
    foreach (var author in authors)
    {
        // Books of each author1 are lazy loaded separately in each loop
        Console.WriteLine(author.Books.Count());
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

    // RemoveRange and UpdateRange are also available
    _context.AddRange(newAuthors);
    
    _context.SaveChanges();
}

// DeleteAnAuthor1();
void DeleteAnAuthor1()
{
    var author = _context.Authors.Find(1);
    _context.Authors.Remove(author);
    _context.SaveChanges();
}

// BulkImmediateDelete();
void BulkImmediateDelete()
{
    // Deletes multiple objects immediately
    int totalNumberOfRowsDeleted = _context.Books.Where(x => x.BookId > 10).ExecuteDelete();
}

// BulkImmediateUpdate();
void BulkImmediateUpdate()
{
    // Updates multiple objects 
    int totalNumberOfRowsUpdated = _context.Authors.Where(x => x.AuthorId > 15)
        .ExecuteUpdate(x => x.SetProperty(x => x.FirstName, "Mehmet"));
}

// CoordinatedRetrieveAndUpdateAuthor();
void CoordinatedRetrieveAndUpdateAuthor()
{
    using var context1 = new PubContext();

    var author = context1.Authors.Find(3);

    author.FirstName = "Julia";

    using var context2 = new PubContext();

    // Although the object is not 
    context2.Authors.Update(author);
    
    context1.ChangeTracker.DetectChanges();

    _ = context1.ChangeTracker.DebugView.ShortView;
    _ = context2.ChangeTracker.DebugView.ShortView;

    context2.SaveChanges();
}

// RetrieveAndUpdateMultipleAuthors();
void RetrieveAndUpdateMultipleAuthors()
{
    var authors = _context.Authors.Where(a => a.LastName.Contains("a")).ToList();
    
    var a1 = authors[0];
    var a2 = authors[1];
    a1 = null;

    _ = _context.ChangeTracker.DebugView.ShortView;

    _context.ChangeTracker.DetectChanges();

    authors.RemoveAt(0);

    _context.ChangeTracker.DetectChanges();
}


// AddBook();
void AddBook()
{
    var book = new Book { Title = "How to crash your app" };
    _context.Books.Add(book);

    // AuthorId is required so this will fail
    _context.SaveChanges();
}


// CascadeDeleteInActionWhenTracked();
void CascadeDeleteInActionWhenTracked()
{
    var author = _context.Authors.Include(a => a.Books)
        .Where(x => x.Books.Count > 0)
        .FirstOrDefault();

    _context.Authors.Remove(author);
    _context.ChangeTracker.DetectChanges();
    var state = _context.ChangeTracker.DebugView.ShortView;
    //_context.SaveChanges();
}

//RemovingRelatedData();
void RemovingRelatedData()
{
    var author = _context.Authors.Include(a => a.Books)
        .Where(x => x.Books.Count > 0)
        .FirstOrDefault();

    var book = author.Books[0];
    author.Books.Remove(book);

    _context.ChangeTracker.DetectChanges();
    var state = _context.ChangeTracker.DebugView.ShortView;
}


//ModifyingRelatedDataWhenNotTracked();

void ModifyingRelatedDataWhenNotTracked()
{
    var author = _context.Authors.Include(a => a.Books)
        .Where(x => x.Books.Count > 0)
        .FirstOrDefault();

    author.Books[0].BasePrice = (decimal)12.00;

    var newContext = new PubContext();
    //newContext.Books.Update(author1.Books[0]);
    newContext.Entry(author.Books[0]).State = EntityState.Modified;
    var state = newContext.ChangeTracker.DebugView.ShortView;
    newContext.SaveChanges();
}

// AddNewBookToExistingAuthorInMemoryViaBook();
void AddNewBookToExistingAuthorInMemoryViaBook()
{
    var book = new Book
    {
        Title = "Shift",
        PublishDate = new DateTime(2012, 1, 1),
        // AuthorId = 5
    };
    book.Author = _context.Authors.First();
    _context.Books.Add(book);
    _context.SaveChanges();
}

// UnAssignAnArtistFromACover();
void UnAssignAnArtistFromACover()
{
    var coverWithArtist = _context.Covers
        .Include(c => c.Artists)
        .Where(a => a.Artists.Count > 0)
        .First();
    coverWithArtist.Artists.RemoveAt(0);
    // _context.Artists.Remove(coverWithArtist.Artists[0]);
    _context.ChangeTracker.DetectChanges();
    var debugview = _context.ChangeTracker.DebugView.ShortView;
    //_context.SaveChanges();
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



// DeleteCover(10);

void DeleteCover(int coverId)
{
    var rowCount = _context.Database.ExecuteSqlRaw("DeleteCover {0}", coverId);
    Console.WriteLine(rowCount);
}

//SimpleRawSQL();
void SimpleRawSQL()
{
    var authors = _context.Authors.FromSqlRaw("select * from authors").OrderBy(a => a.LastName).ToList();
}

// GetAuthorsByArtist();

void GetAuthorsByArtist()
{
    // AuthorsByArtist is a view
    
    var authorartists = _context.AuthorsByArtist.ToList();
    var oneauthorartists = _context.AuthorsByArtist.FirstOrDefault();
    var Kauthorartists = _context.AuthorsByArtist
                                 .Where(a => a.Artist.StartsWith("K")).ToList();

    // Views are not tracked
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


//StringFromInterpolated_Safe();
void StringFromInterpolated_Safe()
{
    var lastnameStart = "L";
    var authors = _context.Authors
        .FromSqlInterpolated($"SELECT * FROM authors WHERE lastname LIKE '{lastnameStart}%'")
    .OrderBy(a => a.LastName).TagWith("Interpolated_Safe").ToList();
}
