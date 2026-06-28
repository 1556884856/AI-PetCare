<!--
  管理仪表盘页面
  展示今日预约数、待确认数、今日营收、本月新客户等关键指标
  同时展示今日预约列表
-->
<template>
  <div><h1 class="admin-title">📊 仪表盘</h1>
    <div class="stat-grid">
      <div class="stat-card"><div class="stat-label">今日预约</div><div class="stat-value">{{ dashboard?.stats?.todayAppointments || 0 }}</div></div>
      <div class="stat-card"><div class="stat-label">待确认</div><div class="stat-value">{{ dashboard?.stats?.pendingAppointments || 0 }}</div></div>
      <div class="stat-card"><div class="stat-label">今日营收</div><div class="stat-value">¥{{ (dashboard?.stats?.todayRevenue || 0).toFixed(2) }}</div></div>
      <div class="stat-card"><div class="stat-label">本月新客户</div><div class="stat-value">{{ dashboard?.stats?.monthNewCustomers || 0 }}</div></div>
    </div>
    <h2 style="margin: 32px 0 16px; font-size: 18px;">📅 今日预约</h2>
    <el-table :data="dashboard?.todayAppointments || []" v-loading="loading" style="width: 100%">
      <el-table-column label="时段" width="130" prop="timeSlot" />
      <el-table-column label="客户" width="120" prop="customerName" />
      <el-table-column label="宠物" width="120"><template #default="{ row }">{{ row.petType === 'Cat' ? '🐈' : '🐕' }} {{ row.petName }}</template></el-table-column>
      <el-table-column prop="serviceName" label="服务" />
      <el-table-column label="状态" width="100">
        <template #default="{ row }"><el-tag v-if="row.status === 0" type="warning">待确认</el-tag><el-tag v-else-if="row.status === 1" type="primary">已确认</el-tag><el-tag v-else-if="row.status === 2" type="success">已完成</el-tag><el-tag v-else type="info">已取消</el-tag></template>
      </el-table-column>
    </el-table>
  </div>
</template>
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getDashboard } from '@/api/admin'
const dashboard = ref<any>(null); const loading = ref(false)
onMounted(async () => { loading.value = true; try { const res: any = await getDashboard(); dashboard.value = res.data } finally { loading.value = false } })
</script>
<style scoped>
.admin-title { font-size: 24px; margin-bottom: 24px; color: #333; }
.stat-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px; }
.stat-card { background: #fff; border-radius: 8px; padding: 24px; box-shadow: 0 2px 8px rgba(0,0,0,0.06); text-align: center; }
.stat-label { font-size: 14px; color: #999; margin-bottom: 8px; }
.stat-value { font-size: 28px; font-weight: 700; color: #333; }
@media (max-width: 768px) { .stat-grid { grid-template-columns: repeat(2, 1fr); } }
</style>
