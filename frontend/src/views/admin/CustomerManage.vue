<!--
  客户管理页面（管理员）
  展示客户列表，含手机号、昵称、宠物数、预约数等
-->
<template>
  <div><h1 class="admin-title">👥 客户管理</h1>
    <el-table :data="list" v-loading="loading" style="width: 100%;">
      <el-table-column prop="phone" label="手机号" width="140" />
      <el-table-column prop="nickname" label="昵称" />
      <el-table-column label="角色" width="100"><template #default="{ row }">{{ row.role === 2 ? '管理员' : row.role === 1 ? '店员' : '客户' }}</template></el-table-column>
      <el-table-column prop="petCount" label="宠物数" width="80" />
      <el-table-column prop="appointmentCount" label="预约数" width="80" />
      <el-table-column label="注册时间" width="180"><template #default="{ row }">{{ formatTime(row.createdAt) }}</template></el-table-column>
    </el-table>
  </div>
</template>
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getCustomers } from '@/api/admin'
const list = ref<any[]>([]); const loading = ref(false)
onMounted(async () => { loading.value = true; try { const res: any = await getCustomers(); list.value = res.data || [] } finally { loading.value = false } })
const formatTime = (t: string) => { const d = new Date(t); return d.toLocaleDateString() + ' ' + d.toLocaleTimeString() }
</script>
<style scoped>
.admin-title { font-size: 24px; margin-bottom: 24px; color: #333; }
</style>
