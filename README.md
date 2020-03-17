# Rami.WebApi.Core
.NET CORE WebApi，API for wechat/weixin


项目预览地址：
http://wechat.tamakirami.top

测试账号密码：admin/admin<br>
服务器为谷歌云：Centos7 nginx 1核 1.7g 10g hdd<br>

项目框架基于anjoy8 https://github.com/anjoy8/Blog.Core 修改<br>
图文编辑器UEditor基于chenderong https://github.com/chenderong/UEditorNetCore 修改<br>


基于.NETCORE 3.1 WEBAPI的公众号管理后台 <br>
公众号类库 https://github.com/tamakiramimy/Rami.Wechat.Core <br>
配合 https://github.com/tamakiramimy/Rami.Wechat.Vue 前端使用，能快速搭建公众号管理后台 <br>

#使用说明<br>

#还原nuget、编译、运行 
dotnet restore <br>
dotnet build <br>
dotnet run <br>

#配置好 公众号配置文件 Wechat.json 相关参数<br>

#如果vue项目和webapi单独部署，设置appsettings.json StartUrl 为 swagger 可以查看api列表 <br>
#如果vue项目和webapi一起部署，设置appsettings.json StartUrl 为 backstage.html 可以修改首页为公众号后台管理登录界面 <br>

#创建本地mysql数据库 #修改appsettings.json 数据库连接，修改IsDbSeedEnabled 为 true； <br>
#调用api/Permission/SeedData 生成库表 <br>
#修改IsDbSeedEnabled 为 false； <br>

#执行项目里面ReadMe.md的数据还原脚本下脚本生成基础数据 <br>
