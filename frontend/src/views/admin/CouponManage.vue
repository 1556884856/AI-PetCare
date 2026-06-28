<!--
  优惠券管理页面（管理员）
  列表展示 + 弹窗新增/编辑/删除优惠券 + 分发功能
-->
<template>
  <div><h1 class="admin-title">🎫 优惠券管理</h1>
    <div class="toolbar"><el-button type="primary" @click="showDialog = true; resetForm()">+ 创建优惠券</el-button></div>
    <el-table :data="list" v-loading="loading" style="width: 100%;">
      <el-table-column prop="name" label="名称" />
      <el-table-column label="类型" width="100"><template #default="{ row }">{{ row.type === 0 ? '满减券' : '折扣券' }}</template></el-table-column>
      <el-table-column label="面值" width="100"><template #default="{ row }">{{ row.type === 0 ? '¥' + row.value : (row.value * 10).toFixed(1) + '折' }}</template></el-table-column>
      <el-table-column label="最低消费" width="100"><template #default="{ row }">{{ row.minOrderAmount > 0 ? '¥' + row.minOrderAmount : '无门槛' }}</template></el-table-column>
      <el-table-column label="有效期" width="200"><template #default="{ row }">{{ formatDate(row.validFrom) }} ~ {{ formatDate(row.validTo) }}</template></el-table-column>
      <el-table-column label="已领/总量" width="100"><template #default="{ row }">{{ row.usedCount }} / {{ row.totalQuantity }}</template></el-table-column>
      <el-table-column label="状态" width="80"><template #default="{ row }"><el-tag :type="row.isActive ? 'success' : 'info'">{{ row.isActive ? '启用' : '禁用' }}</el-tag></template></el-table-column>
      <el-table-column label="操作" width="220">
        <template #default="{ row }"><el-button size="small" @click="editCoupon(row)">编辑</el-button><el-button size="small" type="primary" @click="distribute(row)">分发</el-button><el-button size="small" type="danger" @click="remove(row.id)">删除</el-button></template>
      </el-table-column>
    </el-table>
  </div>
  <el-dialog v-model="showDialog" :title="editingId ? '编辑优惠券' : '创建优惠券'" width="480px">
    <el-form :model="form" label-position="top">
      <el-form-item label="名称"><el-input v-model="form.name" /></el-form-item>
      <el-form-item label="类型"><el-radio-group v-model="form.type"><el-radio :value="0">满减券</el-radio><el-radio :value="1">折扣券</el-radio></el-radio-group></el-form-item>
      <el-form-item :label="form.type === 0 ? '面值(元)' : '折扣(如0.85=85折)'"><el-input-number v-model="form.value" :min="0.01" :step="form.type === 0 ? 1 : 0.05" :precision="2" /></el-form-item>
      <el-form-item label="最低消费金额"><el-input-number v-model="form.minOrderAmount" :min="0" /></el-form-item>
      <el-form-item label="有效期起始"><el-date-picker v-model="form.validFrom" type="datetime" /></el-form-item>
      <el-form-item label="有效期截止"><el-date-picker v-model="form.validTo" type="datetime" /></el-form-item>
      <el-form-item label="发放总量"><el-input-number v-model="form.totalQuantity" :min="1" /></el-form-item>
      <el-form-item label="状态"><el-switch v-model="form.isActive" active-text="启用" inactive-text="禁用" /></el-form-item>
    </el-form>
    <template #footer><el-button @click="showDialog = false">取消</el-button><el-button type="primary" @click="save">保存</el-button></template>
  </el-dialog>
  <el-dialog v-model="showDistribute" title="分发优惠券" width="420px">
    <p>确定将优惠券「{{ distCoupon?.name }}」分发给已选用户？</p>
    <p style="color: #999; font-size: 13px;">分发将给当前所有用户各发一张</p>
    <template #footer><el-button @click="showDistribute = false">取消</el-button><el-button type="primary" @click="doDistribute">确定分发</el-button></template>
  </el-dialog>
</template>
<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { getAllCoupons, createCoupon, updateCoupon, deleteCoupon, distributeCoupon, getCustomers } from '@/api/coupons'
import { ElMessage, ElMessageBox } from 'element-plus'
const list = ref<any[]>([]); const loading = ref(false); const showDialog = ref(false); const showDistribute = ref(false); const editingId = ref<number | null>(null); const distCoupon = ref<any>(null)
const form = reactive({ name: '', type: 0, value: 10, minOrderAmount: 0, validFrom: new Date().toISOString(), validTo: new Date(Date.now() + 7 * 86400000).toISOString(), totalQuantity: 100, isActive: true })
const resetForm = () => { editingId.value = null; Object.assign(form, { name: '', type: 0, value: 10, minOrderAmount: 0, validFrom: new Date().toISOString(), validTo: new Date(Date.now() + 7 * 86400000).toISOString(), totalQuantity: 100, isActive: true }) }
const editCoupon = (c: any) => { editingId.value = c.id; form.name = c.name; form.type = c.type; form.value = c.value; form.minOrderAmount = c.minOrderAmount; form.validFrom = c.validFrom; form.validTo = c.validTo; form.totalQuantity = c.totalQuantity; form.isActive = c.isActive; showDialog.value = true }
const fetchList = async () => { loading.value = true; try { const res: any = await getAllCoupons(); list.value = res.data || [] } finally { loading.value = false } }
onMounted(fetchList)
const save = async () => {
  try {
    if (editingId.value) await updateCoupon(editingId.value, form); else await createCoupon(form)
    ElMessage.success('保存成功'); showDialog.value = false; fetchList()
  } catch {}
}
const remove = async (id: number) => { await ElMessageBox.confirm('确定删除该优惠券吗？', '提示', { type: 'warning' }); try { await deleteCoupon(id); ElMessage.success('已删除'); fetchList() } catch {} }
const distribute = (c: any) => { distCoupon.value = c; showDistribute.value = true }
const doDistribute = async () => {
  if (!distCoupon.value) return
  try {
    // 获取所有用户 ID
    const usersRes: any = await getCustomers()
    const userIds = (usersRes.data || []).map((u: any) => u.id)
    await distributeCoupon(distCoupon.value.id, { userIds })
    ElMessage.success('分发成功'); showDistribute.value = false; fetchList()
  } catch {}
}
const formatDate = (t: string) => new Date(t).toLocaleDateString()
</script>
<style scoped>
.admin-title { font-size: 24px; margin-bottom: 24px; color: #333; }
.toolbar { margin-bottom: 16px; }
</style>
