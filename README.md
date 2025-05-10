# Siska.Admin

Aplikasi dasar untuk agen perjalanan, yang bertujuan membantu admin agen perjalanan mengelola data pengguna. Aplikasi ini dikembangkan menggunakan Clean Architecture untuk memastikan skalabilitas dan pemisahan tanggung jawab dalam kode.

## ?? Tujuan Proyek

Proyek ini dirancang untuk:
- Menyediakan platform bagi admin agen perjalanan untuk mengelola daftar pengguna.
- Membangun fondasi yang dapat diperluas untuk fitur-fitur lainnya di masa depan.

## ?? Teknologi yang Digunakan

- **Bahasa Pemrograman**: C#
- **Framework**: .NET Core 8
- **ORM**: Entity Framework untuk integrasi dengan database
- **Database**: PostgreSQL
- **IDE**: Visual Studio 2022
- **Clean Architecture**: Memisahkan logika aplikasi menjadi beberapa layer untuk keterbacaan dan pemeliharaan yang lebih baik.

## ?? Struktur Proyek

Proyek ini menggunakan pendekatan Clean Architecture dengan layer berikut:
- **Application**: Berisi logika bisnis dan antarmuka aplikasi.
- **Cache**: Untuk caching data sementara.
- **Client**: Untuk komunikasi dengan komponen eksternal jika diperlukan.
- **Database**: Berisi konfigurasi dan konteks database.
- **Model**: Berisi definisi entitas atau model domain.
- **Server**: Berisi konfigurasi dan pengaturan server.
- **Storage**: Untuk manajemen file atau penyimpanan tambahan.
- **Utility**: Berisi utilitas atau helper yang digunakan di seluruh aplikasi.

## ?? Cara Membuild dan Menjalankan Proyek

### Prasyarat
1. Pastikan **.NET Core 8 SDK** sudah terinstal.
2. Pastikan **PostgreSQL** terinstal dan dapat diakses.
3. Gunakan **Visual Studio 2022** untuk membuka dan menjalankan proyek.

### Langkah-Langkah Build
1. Buka file `Siska.Admin.sln` di Visual Studio 2022.
2. Pilih target build (misalnya, Debug atau Release) di toolbar.
3. Tekan `Ctrl+Shift+B` untuk melakukan build proyek.

### Menjalankan Proyek
1. Konfigurasikan string koneksi di file `appsettings.json` dengan kredensial PostgreSQL Anda.
2. Tekan `F5` untuk menjalankan proyek dalam mode debug.
3. Akses aplikasi melalui endpoint yang ditentukan (default: `http://localhost:5000`).

## ?? Cara Debugging

1. Buka Visual Studio 2022 dan pilih konfigurasi Debug.
2. Gunakan breakpoints pada kode untuk memeriksa logika atau troubleshoot masalah.
3. Jalankan aplikasi dengan menekan `F5` dan gunakan **Watch**, **Locals**, atau **Call Stack** untuk menganalisis eksekusi.

## ?? Catatan

- Proyek ini adalah dasar yang dapat diperluas untuk aplikasi agen perjalanan.
- Fitur saat ini hanya mencakup pengelolaan daftar pengguna.
- Pastikan semua dependensi diinstal sebelum menjalankan proyek.

## ?? Lisensi

Proyek ini tidak memiliki lisensi resmi. Jika ingin menggunakannya untuk produksi, harap hubungi pengembang.




## release in docker

go to 'Siska.Admin.Server' folder

build app in release, and put it in 'publish' folder (https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-publish)

`dotnet publish -c Release -o publish`

build docker

`docker build --no-cache -t siska.admin .`

run docker (if you already have postgres running in docker, or set appropriate connection)
`docker run -d --name siska --network postgres_default -p 8080:7777 siska.admin -e ConnectionStrings:DefaultConnection="Host=postgres;Database=siskaadmin;Username=postgres;Password=postgres"`

## release using docker compose

start docker with app and databases

`docker-compose up -d`

drop app and database

`docker-compose down -d`


## NOTES

user admin
 user => "Admin"
 pass => "admin@siska.com"

Microsoft.AspNetCore.Identity, Microsoft.AspNetCore.http => depreceated

https://stackoverflow.com/questions/76849802/how-to-replace-microsoft-aspnetcore-http
 intinya tambahkan di item group ==> <FrameworkReference Include="Microsoft.AspNetCore.App" />