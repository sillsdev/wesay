apt-get -y install ruby1.9.1 ruby1.9.1-dev libxml2-dev libxslt-dev
gem install bundler
mkdir -p ~/src
cd ~/src
git clone https://github.com/chrisvire/BuildUpdate
cd ~/src/BuildUpdate
bundle install
bundle exec gem pristine nokogiri
echo "~/src/BuildUpdate ready"
