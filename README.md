# Draw-A-Line

在unity中尝试使用网格拟合直线

目前实现了两种算法：
1. 插值法，即在线段中选择一些采样点，用采样点所在的格子拟合直线
2. 扫描法（我自己瞎起的名字），从起点开始沿着线段方向，类似bfs，把线段经过的所有格子全用来拟合直线

效果如下：

![插值法](https://cdn.jsdelivr.net/gh/FcAYH/Images/2022/10/17/f544214be590fe15b2743ea6172ba448.png)

![扫描法](https://cdn.jsdelivr.net/gh/FcAYH/Images/2022/10/17/936f3e5ccd1d7e30daf3b1a142241034.png)

发现在这个网站介绍了很多相关算法，打算尝试都去实现一下 ：[www.edepot.com/algorithm.html](http://www.edepot.com/algorithm.html)
