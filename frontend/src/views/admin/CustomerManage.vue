<template>
  <div>
    <h1 class="admin-title">👥 客户管理</h1>
    <el-table :data="list" v-loading="loading" style="width: 100%">
      <el-table-column prop="phone" label="手机号" width="140" />
      <el-table-column prop="nickname" label="昵称" />
      <el-table-column label="角色" width="100">
        <template #default="{ row }">
          <el-tag size="small" :type="row.role === 2 ? 'warning' : 'info'">{{ row.role === 2 ? '管理员' : row.role === 1 ? '店员' : '顾客' }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="petCount" label="宠物数" width="100" />
      <el-table-column prop="appointmentCount" label="预约数" width="100" />
      <el-table-column prop="createdAt" label="注册时间" width="180" />
    </el-table>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getCustomers } from '@/api/admin'

const list = ref<any[]>([])
const loading = ref(false)

onMounted(async () => {
  loading.value = true
  try { const res: any = await getCustomers(); list.value = res.data || [] } catch {} finally { loading.value = false }
})
</script>

<style scoped>
.admin-title { font-size: 24px; margin-bottom: 24px; }
</style>
