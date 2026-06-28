<!--
  预约管理页面（管理员）
  按日期和状态筛选预约列表，支持确认、完成、取消操作
-->
<template>
  <div><h1 class="admin-title">📅 预约管理</h1>
    <div class="filters">
      <el-date-picker v-model="filterDate" type="date" placeholder="选择日期" clearable @change="fetchList" />
      <el-select v-model="filterStatus" placeholder="选择状态" clearable @change="fetchList" style="width: 140px;">
        <el-option label="全部" :value="undefined" /><el-option label="待确认" :value="0" /><el-option label="已确认" :value="1" /><el-option label="已完成" :value="2" /><el-option label="已取消" :value="3" />
      </el-select>
    </div>
    <el-table :data="list" v-loading="loading" style="width: 100%; margin-top: 16px;">
      <el-table-column label="日期" width="120"><template #default="{ row }">{{ formatDate(row.appointmentDate) }}</template></el-table-column>
      <el-table-column prop="timeSlot" label="时段" width="130" />
      <el-table-column label="客户" prop="customerName" width="100" />
      <el-table-column label="宠物" width="120"><template #default="{ row }">{{ row.petType === 'Cat' ? '🐈' : '🐕' }} {{ row.petName }}</template></el-table-column>
      <el-table-column prop="serviceName" label="服务" />
      <el-table-column label="状态" width="100">
        <template #default="{ row }"><el-tag v-if="row.status === 0" type="warning">待确认</el-tag><el-tag v-else-if="row.status === 1" type="primary">已确认</el-tag><el-tag v-else-if="row.status === 2" type="success">已完成</el-tag><el-tag v-else type="info">已取消</el-tag></template>
      </el-table-column>
      <el-table-column label="操作" width="260">
        <template #default="{ row }">
          <el-button v-if="row.status === 0" size="small" type="primary" @click="confirm(row.id)">确认</el-button>
          <el-button v-if="row.status === 0 || row.status === 1" size="small" type="success" @click="complete(row.id)">完成</el-button>
          <el-button v-if="row.status !== 3" size="small" type="danger" @click="cancel(row.id)">取消</el-button>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue'
import { getAllAppointments, confirmAppointment, completeAppointment, adminCancelAppointment } from '@/api/appointments'
import { ElMessage, ElMessageBox } from 'element-plus'
const list = ref<any[]>([]); const loading = ref(false); const filterDate = ref(''); const filterStatus = ref<number | undefined>(undefined)
const fetchList = async () => {
  loading.value = true
  try { const params: any = {}; if (filterDate.value) params.date = filterDate.value; if (filterStatus.value !== undefined) params.status = filterStatus.value; const res: any = await getAllAppointments(params); list.value = res.data || [] } finally { loading.value = false }
}
fetchList()
const confirm = async (id: number) => { try { await confirmAppointment(id); ElMessage.success('已确认'); fetchList() } catch {} }
const complete = async (id: number) => { try { await completeAppointment(id); ElMessage.success('已完成'); fetchList() } catch {} }
const cancel = async (id: number) => { await ElMessageBox.confirm('确定取消该预约吗？', '提示', { type: 'warning' }); try { await adminCancelAppointment(id); ElMessage.success('已取消'); fetchList() } catch {} }
const formatDate = (val: string) => val ? val.slice(0, 10) : ''
</script>
<style scoped>
.admin-title { font-size: 24px; margin-bottom: 24px; color: #333; }
.filters { display: flex; gap: 16px; }
</style>
