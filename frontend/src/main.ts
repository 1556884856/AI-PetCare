// ===================================================================
// 应用入口文件
// 初始化 Vue 实例，注册 Pinia、Vue Router、Element Plus 及其图标
// ===================================================================
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import zhCn from 'element-plus/dist/locale/zh-cn.mjs'  // Element Plus 中文本地化
import * as ElementPlusIconsVue from '@element-plus/icons-vue'
import App from './App.vue'
import router from './router'
import './assets/global.css'
const app = createApp(App)
app.use(createPinia())               // 状态管理
app.use(router)                      // 路由
app.use(ElementPlus, { locale: zhCn }) // UI 组件库（中文）
// 全局注册所有 Element Plus 图标
for (const [key, component] of Object.entries(ElementPlusIconsVue)) { app.component(key, component) }
app.mount('#app')
