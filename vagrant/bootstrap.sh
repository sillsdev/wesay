#!/usr/bin/env bash

apt-get -y install python-software-properties pkg-config
add-apt-repository 'deb http://packages.sil.org/ubuntu precise main'
add-apt-repository 'deb http://packages.sil.org/ubuntu precise-experimental main'
wget -O - http://packages.sil.org/sil.gpg | sudo apt-key add -
apt-get update
apt-get -y install geckofx xulrunner-geckofx mono-sil mono-basic-sil libgdiplus-sil gtk-sharp2-sil libimage-exiftool-perl chmsee git curl
