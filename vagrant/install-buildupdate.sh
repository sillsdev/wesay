sudo apt-get -y install ruby1.9.1 ruby1.9.1-dev libxml2-dev libxslt-dev
sudo gem install bundler
mkdir -p ~/dev
cd ~/dev
git clone https://github.com/chrisvire/BuildUpdate
cd ~/dev/BuildUpdate
bundle install
bundle exec gem pristine nokogiri
