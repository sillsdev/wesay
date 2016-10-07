cd "$(dirname "$0")/.."
root=$PWD
mkdir -p ~/src
cd ~/src
git clone --reference $root/.git -b develop git://gerrit.lsdev.sil.org/wesay
echo "~/src/wesay ready"
