chcp 65001

title "GAME_SERVER 游戏服务器"

@java -javaagent:lib/common-1.0.jar -cp "./;lib/*" -Xms128m -Xmx2g -Xrunjdwp:transport=dt_socket,address=*:5005,server=y,suspend=n -Dfile.encoding=UTF-8 -Djava.security.auth.login.config=D:\work\immortal\server\runtime\kafka_client_jaas.conf -Dio.netty.leakDetection.level=advanced gs.Main gs.properties