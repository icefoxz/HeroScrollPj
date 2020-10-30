#!/bin/sh
# Use -Dlog4j.debug for Log4J startup debugging info
# Use -Xms512M -Xmx512M to start with 512MB of heap memory. Set size according to your needs.
# Use -XX:+CMSClassUnloadingEnabled -XX:+CMSPermGenSweepingEnabled for PermGen GC

JAVA_CMD="java"
localhost="127.0.0.1"
# moudle must be unique
MODULE=gs_1001
CPATH="./:lib/*"

######
# check param
######
if [ "$1" == "" ] || [ "$1" == "help" ] ; then
        echo "run.sh [start/stop/restart/help]"
        echo ""
        echo "start   : start game server   "
        echo "stop    : stop game server    "
        echo "restart : restart game server "
        echo ""
        echo "for example:"
        echo "          run.sh start"
        exit 0
fi

######
# stop
######
if [ "$1" == "stop" ] || [ "$1" == "restart" ] ; then
        echo "stop server: [ $MODULE ]"
        echo "kill server process"

        pid=$(ps ax | grep "module.name=$MODULE " | grep -v grep | awk '{print $1}')

if [ "$pid" == "" ] ; then
        echo "kill ok"
else
        ps ax | grep "module.name=$MODULE " | grep -v grep | awk '{print $1}' |xargs kill -15
        pid=$(ps ax | grep "module.name=$MODULE " | grep -v grep | awk '{print $1}')
        
        count=0
        flag=1
        result=1
        while [ "$flag" -eq 1 ]
        do
           sleep 1s
           result=$(ps ax | grep "module.name=$MODULE " | grep -v grep | awk '{print $1}')
           if [ -z "$result" ]; then
              echo "process $pid is finished"
              flag=0
           fi
           let count+=1;
           if [ "$count" -gt 30 ]; then
              ps ax | grep "module.name=$MODULE " | grep -v grep | awk '{print $1}' |xargs kill -9
              count=0
              echo "kill timeout, use kill -9"
           fi
        done
fi

        if [ "$1" == "stop" ] ; then
                exit 0
        fi
fi

echo "check has the same process: $MODULE"
pid=$(ps ax | grep "module.name=$MODULE " | grep -v grep | awk '{print $1}')

if [ "$pid" == "" ] ; then
  echo "OK"
else
  echo "has same process, please stop process id:$pid"
  exit 0
fi

echo "starting ..."


#nohup ${JAVA_CMD} -javaagent:lib/common-1.0.jar -cp "${CPATH}" -Xmn2g -Xmx6g -Xms6g -XX:TargetSurvivorRatio=90 -XX:MaxTenuringThreshold=10 -Xlog:gc* -Xlog:gc:logs/gc.log -XX:-OmitStackTraceInFastThrow -XX:ReservedCodeCacheSize=256m -XX:+HeapDumpOnOutOfMemoryError  -XX:HeapDumpPath=logs -Dmodule.name=$MODULE -Djava.rmi.server.hostname=${localhost} -Xrunjdwp:transport=dt_socket,address=*:5006,server=y,suspend=n -Dfile.encoding=UTF-8 gs.Main gs.properties >/dev/null 2>&1 &

JMX_PARAM=" -Dcom.sun.management.jmxremote.port=8602 -Dcom.sun.management.jmxremote.authenticate=false -Dcom.sun.management.jmxremote.ssl=false -Dcom.sun.management.jmxremote.rmi.port=8602 -Djava.rmi.server.hostname=127.0.0.1 -Dcom.sun.management.jmxremote.local.only=false "

# nohup ${JAVA_CMD} -javaagent:lib/common-1.0.jar -cp "${CPATH}" -Xmn256M -Xmx1g -Xms1g -XX:TargetSurvivorRatio=90 -XX:MaxTenuringThreshold=10 -Xlog:all=info:file=logs/jvm.log:time,uptime,tid,level,tags:filecount=10,filesize=100M -XX:-OmitStackTraceInFastThrow  -XX:+HeapDumpOnOutOfMemoryError  -XX:HeapDumpPath=logs -Dmodule.name=$MODULE -Djava.rmi.server.hostname=${localhost} -Xrunjdwp:transport=dt_socket,address=*:5005,server=y,suspend=n -Djava.security.auth.login.config=/opt/immortal/kafka/kafka_client_jaas.conf -Dfile.encoding=UTF-8 ${JMX_PARAM} gs.Main gs.properties >/dev/null 2>&1 &

nohup ${JAVA_CMD} -javaagent:lib/common-1.0.jar -cp "${CPATH}" -Xms128m -Xmx2g  -Dmodule.name=$MODULE gs.Main gs.properties >/dev/null 2>&1 &

# java -javaagent:lib/common-1.0.jar -cp "./:lib/*" -Xms128m -Xmx1g -Dmodule.name=$MODULE gs.Main gs.properties 


# tail -F /opt/immotal/gs_1001/logs/gs.log|sed '/Server start successfully/Q'
