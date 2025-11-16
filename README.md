# Car Parts System

A comprehensive ASP.NET Core 8 MVC web application for managing a car parts shop with e-commerce functionality, admin panel, and reporting features.

## 🚀 Features

### Frontend Features
- **Parts Shopping**: Browse, search, and filter car parts by category, brand, and price
- **Shopping Cart**: Add items to cart with AJAX and real-time updates
- **Promotions**: View current deals and special offers
- **Gallery**: Browse images of parts and services
- **Contact**: Company information and contact details
- **User Authentication**: Register, login, and manage account
- **Dark/Light Mode**: Toggle between themes with localStorage persistence

### Admin Features
- **Dashboard**: Overview with statistics (parts, orders, revenue)
- **Parts Management**: Full CRUD operations with image upload
- **Order Management**: View, track, and update order status
- **Promotions Management**: Create and manage promotional campaigns
- **Gallery Management**: Upload and manage gallery images
- **Reports**: 
  - Sales reports by date range
  - Staff sales performance reports
  - Export to PDF (DinkToPdf)

## 🛠 Technology Stack

- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: Microsoft SQL Server (LocalDB/MSSQL)
- **ORM**: Entity Framework Core 8.0
- **Authentication**: ASP.NET Core Identity
- **Frontend**: 
  - Bootstrap 5 (Responsive UI)
  - jQuery (AJAX interactions)
  - Font Awesome 6 (Icons)
  - SweetAlert2 (Notifications)
- **PDF Generation**: DinkToPdf
- **Image Upload**: IFormFile with file system storage

## 📋 Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or full SQL Server)
- Visual Studio 2022 / VS Code / Rider

## 🔧 Installation & Setup

### 1. Clone the Repository
```bash
git clone https://github.com/entaneerxi/CarPartsSystem.git
cd CarPartsSystem
```

### 2. Update Database Connection String
Edit `appsettings.json` and update the connection string if needed:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CarPartsSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 3. Install Dependencies
```bash
cd CarPartsSystem
dotnet restore
```

### 4. Create Database
```bash
# Add initial migration (if not exists)
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

### 5. Run the Application
```bash
dotnet run
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`

## 👥 Default User Accounts

The system comes with pre-seeded accounts for testing:

### Admin Account
- **Email**: admin@carparts.com
- **Password**: Admin123!
- **Role**: Admin (Full access)

### Staff Account
- **Email**: staff@carparts.com
- **Password**: Staff123!
- **Role**: Staff (Limited admin access)

### Test Member Account
Register a new account through the registration page to test member features.

## 📁 Project Structure

```
CarPartsSystem/
├── Controllers/           # MVC Controllers
│   ├── AccountController.cs
│   ├── AdminController.cs
│   ├── AdminPartsController.cs
│   ├── AdminOrdersController.cs
│   ├── AdminPromotionsController.cs
│   ├── AdminGalleryController.cs
│   ├── CartController.cs
│   ├── PartsController.cs
│   ├── PromotionsController.cs
│   ├── GalleryController.cs
│   ├── ContactController.cs
│   └── ReportsController.cs
├── Models/               # Data Models
│   ├── ApplicationUser.cs
│   ├── Part.cs
│   ├── Purchase.cs
│   ├── PurchaseItem.cs
│   ├── CartItem.cs
│   ├── Promotion.cs
│   ├── PaymentMethod.cs
│   ├── PaymentTransaction.cs
│   ├── Gallery.cs
│   ├── Slide.cs
│   ├── ContactInfo.cs
│   └── BookingPostponeRequest.cs
├── Views/                # Razor Views
│   ├── Home/
│   ├── Account/
│   ├── Parts/
│   ├── Cart/
│   ├── Admin/
│   ├── AdminParts/
│   └── Shared/
├── Data/                 # Database Context
│   ├── ApplicationDbContext.cs
│   └── DbInitializer.cs
└── wwwroot/              # Static Files
    ├── css/
    ├── js/
    └── uploads/          # Uploaded images
```

## 🎨 Key Features Implementation

### Dark/Light Mode
The theme toggle is implemented using Bootstrap 5's data-bs-theme attribute:
- Theme preference stored in localStorage
- JavaScript in _Layout.cshtml handles theme switching
- Smooth transitions between themes

### Image Upload
Images are stored in `/wwwroot/uploads/` with subfolders for different entities:
- `/uploads/parts/` - Part images
- `/uploads/promotions/` - Promotion banners
- `/uploads/gallery/` - Gallery images
- `/uploads/slides/` - Homepage carousel slides

### Shopping Cart
- AJAX-based cart operations
- Real-time quantity updates
- SweetAlert notifications for user feedback
- Session-based cart for guests, database-backed for authenticated users

### Role-Based Access Control
- **Member**: Can browse parts, add to cart, make purchases
- **Staff**: Member permissions + access to admin panel (limited)
- **Admin**: Full system access including reports and user management

## 📊 Database Schema

The application uses the following main entities:
- **ApplicationUser**: User accounts with Identity
- **Part**: Car parts catalog
- **Purchase**: Orders placed by users
- **PurchaseItem**: Line items in orders
- **CartItem**: Shopping cart items
- **Promotion**: Promotional campaigns
- **PaymentMethod**: Available payment options
- **PaymentTransaction**: Payment records
- **Gallery**: Image gallery
- **ContactInfo**: Company contact information
- **BookingPostponeRequest**: Postponement requests

## 🔐 Security Features

- ASP.NET Core Identity for authentication
- Role-based authorization
- Anti-forgery tokens on forms
- Secure password requirements
- SQL injection protection via EF Core
- XSS protection

## 📱 Responsive Design

The application is fully responsive and works on:
- Desktop computers
- Tablets
- Mobile phones

## 🐛 Troubleshooting

### Database Connection Issues
If you encounter database connection issues:
1. Ensure SQL Server/LocalDB is running
2. Check connection string in `appsettings.json`
3. Run `dotnet ef database update` to apply migrations

### Image Upload Issues
Ensure the upload directories exist and have write permissions:
```bash
mkdir -p wwwroot/uploads/parts
mkdir -p wwwroot/uploads/promotions
mkdir -p wwwroot/uploads/gallery
mkdir -p wwwroot/uploads/slides
```

## 📝 Future Enhancements

- [ ] Complete checkout and payment processing
- [ ] Email notifications for orders
- [ ] PDF invoice generation
- [ ] User purchase history dashboard
- [ ] Booking postponement workflow
- [ ] Payment confirmation workflow
- [ ] Additional payment gateway integrations
- [ ] Multi-language support
- [ ] Product reviews and ratings

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the MIT License.

## 👨‍💻 Author

**Entaneerxi**

## 🙏 Acknowledgments

- Bootstrap Team for the excellent CSS framework
- Font Awesome for icons
- SweetAlert2 for beautiful alerts
- Microsoft for ASP.NET Core
