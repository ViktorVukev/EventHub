using EventHub.DataAccess.Data;
using EventHub.DataAccess.Repository;
using EventHub.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using EventHub.Utility;
using EventHub.Models;
using Microsoft.DiaSymReader;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//Connection to the database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders().AddErrorDescriber<IdentityValidationTranslations>();
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});
builder.Services.AddRazorPages();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
	name: "default",
	pattern: "{area=User}/{controller=Home}/{action=Index}/{id?}");

/* Adding roles */
using (var scope = app.Services.CreateScope())
{
	var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

	var roles = new[] { StaticDetails.Role_User, StaticDetails.Role_Admin };

	foreach (var role in roles)
	{
		if (!await roleManager.RoleExistsAsync(role))
		{
			await roleManager.CreateAsync(new IdentityRole(role));
		}
	}
}

/* Adding admin account */
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

	string email = "admin@eventhub.com";
	string password = "Admin_EventHub_2024";
	string firstName = "Administrator";
	string lastName = "EventHub";

	if (await userManager.FindByEmailAsync(email) == null)
	{
		var user = new ApplicationUser
		{
			UserName = email,
			Email = email,
			FirstName = firstName,
			LastName = lastName
		};

		var result = await userManager.CreateAsync(user, password);

		if (result.Succeeded)
		{
			await userManager.AddToRoleAsync(user, StaticDetails.Role_Admin);
		}
	}
}

app.Run();
