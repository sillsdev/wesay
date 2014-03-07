mkdir -p ~/dev
cd ~/dev
git clone --reference $(dirname "$0")/../.git -b develop git://gerrit.lsdev.sil.org/wesay
cd ~/dev/wesay
