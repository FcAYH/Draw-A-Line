# Draw-A-Line

在unity中尝试使用网格拟合直线

忘记是哪家的笔试题了，考到了这个知识点，当时求的是直线一共经过了多少格子，
当时就想了个插值法，其实对于这个题目插值法是不对的，应该用扫描法。

于是乎就想着自己用unity实现一下这样一个功能。

目前实现了两种算法：
1. 插值法，即在线段中选择一些采样点，用采样点所在的格子拟合直线
2. 扫描法（我自己瞎起的名字），从起点开始沿着线段方向，类似bfs，把线段经过的所有格子全用来拟合直线

效果如下：

![插值法](https://cdn.jsdelivr.net/gh/FcAYH/Images/2022/10/17/48f577f58c6936a2ca82696b89ca4067.png)

![扫描法](https://cdn.jsdelivr.net/gh/FcAYH/Images/2022/10/17/936f3e5ccd1d7e30daf3b1a142241034.png)

在这个网站介绍了一个效率较高？的算法：[www.edepot.com/algorithm.html](http://www.edepot.com/algorithm.html)

目前实现了其三种基本计算方式：Division，Multiplication，Addition

效果如下：

![EFLA](https://cdn.jsdelivr.net/gh/FcAYH/Images/2022/10/17/4a31823698c982b97c8b1f99ff7916e0.png)
