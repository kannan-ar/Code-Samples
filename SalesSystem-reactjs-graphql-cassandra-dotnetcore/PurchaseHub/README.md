docker run -e DS_LICENSE=accept --memory 4g --name my-dse -d datastax/dse-server:6.8.0 -g -s -k
docker run -e DS_LICENSE=accept --link my-dse -p 9091:9091 --memory 1g --name my-studio -d datastax/dse-studio
