# Maintainer: Inzerd djynze@gmail.com
pkgname=passgeneretor  # Nome del pacchetto
pkgver=1.0.0  # Versione del pacchetto
pkgrel=1  # Numero di rilascio del pacchetto
pkgdesc="A simple password generator"  # Descrizione del pacchetto
arch=('x86_64')  # Architettura supportata
url="https://github.com/Inzerd/passgeneretor"  # URL del progetto
license=('MIT')  # Licenza del pacchetto
depends=('dotnet-runtime')  # Dipendenze necessarie per eseguire il pacchetto
makedepends=('dotnet-sdk')  # Dipendenze necessarie per costruire il pacchetto
source=("$pkgname-$pkgver::git+https://github.com/Inzerd/passgeneretor.git")  # Sorgente del codice
sha256sums=('SKIP')  # Checksum del sorgente (SKIP significa che non viene verificato)

build() {
  cd "$srcdir/$pkgname-$pkgver"  # Entra nella directory del sorgente
  dotnet build --configuration Release  # Costruisce il progetto in modalità Release
}

package() {
  cd "$srcdir/$pkgname-$pkgver"  # Entra nella directory del sorgente
  install -Dm755 "bin/Release/net6.0/$pkgname" "$pkgdir/usr/bin/$pkgname"  # Installa il binario nella directory di destinazione
  install -Dm644 LICENSE "$pkgdir/usr/share/licenses/$pkgname/LICENSE"  # Installa il file di licenza
}

# vim:set ts=2 sw=2 et:  # Impostazioni di vim per la formattazione del file