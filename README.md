# Spot

### 多屏图像查看工具

工具包含一个图像查看工具和 `TcpServer` 

## 使用场景

线上有一台主机（服务端，`S`），连接多个或一个屏幕，屏幕用于显示图像

显示图像的事件，来自线上其他设备/主机（客户端，`C`），`C` 通过 `TCP` 协议请求 `S` 在指定的显示屏显示指定的图像

图像文件可以存放在线上任何一台主机，可以是 `C` 自身，也可以是统一的图像文件服务器，但图像文件需要保证 `S` 可访问（通过共享文件夹）

## 如何使用

### 解压

解压后得到如下结构的目录：

```
└── Spot
    ├── appsettings.json
    ├── Spot.exe
    └── 使用说明.pdf
```

### 配置工作

**以下是一个配置范例：**

1. 确定 `S`  连接的屏幕数，这里假设有 `2` 个屏幕
2. 确定 `TcpServer` 的监听端口，这里假设是 `8083`
3. 打开 `appsettings.json`  看到文件如下：

    ```
    {
      "TcpPort": 8087,
      "MonitorSequences": [1,3,2,4]
    }
    ```

​	 	`TcpPort` 选项用于指定 `TcpServer` 监听的端口

​		 `MonitorSequences` 选项用于指定显示器顺序

4. 把原来的 `8087` 修改为 `8083` **（注意：这只是个范例）**

5. 把原来的 `[1,3,2,4]` 修改为 `[1,2]` **（注意：这只是个范例）**

   （因为例子中只有两个显示器，`[1,3,2,4]` 的含义是将 2 号窗口打开在 3 号显示器，这个例子中没有3号显示器，会报错要求修改配置）

6. 现在 `appsettings.json`  中的配置如下：

   ```
   {
     "TcpPort": 8083,
     "MonitorSequences": [1,2]
   }
   ```

   **（注意：配置文件中最后一个 `}` 之前不能有逗号）**

7. 运行 `Spot.exe`，观察窗口顺序是否符合要求，不符合关闭软件后，调整配置中的 `MonitorSequences` 直到符合要求。

> `MonitorSequences` 选项说明
>
> 配置值是一个数组，数字所在的位置代表窗口序号（从1开始），数字表示屏幕序号（从1开始，不保证和windows设置中的屏幕标识一致）
>
> 比如 `[2,3,4,1]` 表示：
> 1 号窗口在 2 号屏幕打开
> 2 号窗口在 3 号屏幕打开
> 3 号窗口在 4 号屏幕打开
> 4 号窗口在 1 号屏幕打开
>
> 
> 限制 1：
> 假设屏幕数量是 n
> 数组前 n 位数字不能大于 n
>
> 比如：
> ​       屏幕数是 3，配置是 `[1,3,4,2]`，这个配置会报错，因为只有 4 号屏幕不存在
> ​       屏幕数是3，配置是 `[2,1,3,4]`，这个配置不会报错，因为前 3 位没有数字大于 3（屏幕数是 3，第四个数字 4 不会被读到）
>
> 
> 限制 2：
> 配置项中不能出现数字 0

### 运行

运行 `Spot.exe` 后，会打开多个或一个图像查看的窗口，以及一个主窗口（写了些操作说明）

**关闭任何一个窗口都会使整个软件退出**

这个时候 `TcpServer` 同时也启动了

### 显示图像

假设 `TcpServer` 的地址是  `192.168.100.77:8083`

（`TcpServer` 可以同时连接多个客户端）

1. 客户端与 `192.168.100.77:8083` 建立 `tcp` 连接

2. 以 `UTF-8` 编码发送

    ```
    窗口序号,图像地址
    ```

   消息，使工具将指定图像加载到对应窗口
   
   比如发送
   
   ```
   3,\\DESKTOP-E3NQ6K3\Shared\pic3.jpg
   ```
   
   工具会将图像 `pic3.jpg` 输出在 3 号窗口显示
   
   **（注意：保证消息中的窗口序号后的逗号是半角的）**
   
   

### 图像查看

- 鼠标滚轮缩放图像
- 按住鼠标右键拖拽图像
- 双击鼠标左键恢复缩放


