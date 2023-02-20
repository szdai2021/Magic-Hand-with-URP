const { Socket } = require('dgram');
const net = require('net');

const robot_port = 21;
const unity_port = 27;

const robot_client_handle = net.createServer().listen(robot_port, () => {
    console.log("Start listening from robot on port " + robot_port);
});


const unity_client_handle = net.createServer().listen(unity_port, () => {
    console.log("Start listening from unity on port " + unity_port);
});


var robot_client = null;
var unity_client = null;

robot_client_handle.on('connection', (robot_client_socket) => {
    robot_client_socket.setEncoding("utf8");
    robot_client= robot_client_socket;
    console.log("Connection: from robot: " + robot_client.remoteAddress + ' : ' + robot_client.remotePort);

    robot_client_socket.on('data', (data) => {
        if(!data.startsWith("i")){
            console.log("receive pos from robot: " + data);
        }
        if (unity_client != null) {
            unity_client.write(data + '\n');
        }
    });

    robot_client_socket.on('error', (err) => {
    })

    robot_client_socket.on('close', (data) => {
        robot_client = null;
    });
});

unity_client_handle.on('connection', (unity_client_socket) => {
    unity_client = unity_client_socket;
    console.log("Connection: from unity:  " + unity_client.remoteAddress + ' : ' + unity_client.remotePort);

    unity_client_socket.on('data', (data) => {
        console.log("receive pos from unity: " + data);
        if (robot_client != null) {
            robot_client.write(data);
        }
    });

    unity_client_socket.on('close', (data) => {
        unity_client = null;
    });
});
