# Maintainer: Inzerd djynze@gmail.com
pkgname=passgeneretor 
pkgver=1.0.0
corever=('net9.0')
pkgrel=1
pkgdesc="A simple password generator"
arch=('x86_64')
url="https://github.com/Inzerd/passGeneretor"
license=('MIT')
depends=()
makedepends=('dotnet-sdk')
source=("$pkgname-$pkgver::git+https://github.com/Inzerd/passGeneretor.git")
sha256sums=('SKIP')

build() {
  cd "$srcdir/$pkgname-$pkgver"
  dotnet restore
  dotnet publish -c Release -r linux-x64 --self-contained true -p:TrimMode=link
}

package() {
  install -Dm755 "$srcdir/$pkgname-$pkgver/bin/Release/$corever/linux-x64/publish/$pkgname" "$pkgdir/usr/bin/$pkgname"
  install -Dm644 "$srcdir/$pkgname-$pkgver/LICENSE" "$pkgdir/usr/share/licenses/$pkgname/LICENSE"
}

package_passgenerator() {
        pkgdesc="A simple password generator"
        depends=()
        conflicts=()
        provides=("passgenerator")
        replaces=()
        arch=('x86_64')
}
